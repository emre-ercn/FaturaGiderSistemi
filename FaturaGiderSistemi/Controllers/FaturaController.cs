using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FaturaGiderSistemi.Data;
using FaturaGiderSistemi.Models;
using System.Linq;
using System.Threading.Tasks;

namespace FaturaGiderSistemi.Controllers
{
    [Authorize]
    public class FaturalarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaturalarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Faturalar
        public async Task<IActionResult> Index(string aramaKelimesi, string durumFiltresi)
        {
            var faturalar = _context.Faturalar.Include(f => f.Sirket).AsQueryable();

            if (!string.IsNullOrEmpty(aramaKelimesi))
            {
                faturalar = faturalar.Where(f => f.FaturaNo.Contains(aramaKelimesi));
            }

            if (!string.IsNullOrEmpty(durumFiltresi))
            {
                bool odendiMi = durumFiltresi == "1";
                faturalar = faturalar.Where(f => f.Durum == odendiMi);
            }

            ViewBag.AramaKelimesi = aramaKelimesi;
            ViewBag.DurumFiltresi = durumFiltresi;

            return View(await faturalar.ToListAsync());
        }

        // GET: Faturalar/Create
        public IActionResult Create()
        {
            ViewData["SirketId"] = new SelectList(_context.Sirketler, "Id", "Ad");
            return View();
        }

        // POST: Faturalar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fatura fatura)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fatura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SirketId"] = new SelectList(_context.Sirketler, "Id", "Ad", fatura.SirketId);
            return View(fatura);
        }

        // GET: Faturalar/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var fatura = await _context.Faturalar.FindAsync(id);
            if (fatura == null) return NotFound();

            ViewData["SirketId"] = new SelectList(_context.Sirketler, "Id", "Ad", fatura.SirketId);
            return View(fatura);
        }

        // POST: Faturalar/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Fatura fatura)
        {
            if (id != fatura.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(fatura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SirketId"] = new SelectList(_context.Sirketler, "Id", "Ad", fatura.SirketId);
            return View(fatura);
        }

        // GET: Faturalar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var fatura = await _context.Faturalar
                .Include(f => f.Sirket)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fatura == null) return NotFound();

            return View(fatura);
        }

        // POST: Faturalar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fatura = await _context.Faturalar.FindAsync(id);
            if (fatura != null)
            {
                _context.Faturalar.Remove(fatura);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // EXCEL'E AKTARMA METODU (Şimdi sınıfın içinde ve güvende!)
        public IActionResult ExcelaAktar()
        {
            var faturalar = _context.Faturalar.Include(f => f.Sirket).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Faturalar Listesi");

                worksheet.Cell(1, 1).Value = "Fatura No";
                worksheet.Cell(1, 2).Value = "Fiş No";
                worksheet.Cell(1, 3).Value = "Şirket Adı";
                worksheet.Cell(1, 4).Value = "Toplam Tutar";
                worksheet.Cell(1, 5).Value = "Durum";
                worksheet.Cell(1, 6).Value = "Tarih";

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 2;
                foreach (var fatura in faturalar)
                {
                    worksheet.Cell(row, 1).Value = fatura.FaturaNo;
                    worksheet.Cell(row, 2).Value = fatura.FisNo;
                    worksheet.Cell(row, 3).Value = fatura.Sirket != null ? fatura.Sirket.Ad : "Bilinmiyor";
                    worksheet.Cell(row, 4).Value = fatura.ToplamTutar;
                    worksheet.Cell(row, 5).Value = fatura.Durum ? "Ödendi" : "Bekliyor";
                    worksheet.Cell(row, 6).Value = fatura.Tarih.ToString("dd.MM.yyyy");
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Fatura_Listesi.xlsx");
                }
            }
        }
    }
}
using ClosedXML.Excel;
using FaturaGiderSistemi.Data;
using FaturaGiderSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
// Kendi projenin Models klasörünü buraya using ile eklediğinden emin ol (Örn: using FaturaMasrafSistemi.Models;)

namespace FaturaMasrafSistemi.Controllers
{
    public class FaturalarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaturalarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. LİSTELEME EKRANI ---
        public async Task<IActionResult> Index()
        {
            var faturalar = await _context.Faturalar.Include(f => f.Sirket).ToListAsync();
            return View(faturalar);
        }

        // --- 2. FATURA EKLEME ---
        public IActionResult Create()
        {
            ViewBag.SirketId = new SelectList(_context.Sirketler, "Id", "Ad");
            return View();
        }

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
            ViewBag.SirketId = new SelectList(_context.Sirketler, "Id", "Ad", fatura.SirketId);
            return View(fatura);
        }

        // --- 3. FATURA DÜZENLEME ---
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var fatura = await _context.Faturalar.FindAsync(id);
            if (fatura == null) return NotFound();

            ViewBag.SirketId = new SelectList(_context.Sirketler, "Id", "Ad", fatura.SirketId);
            return View(fatura);
        }

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
            ViewBag.SirketId = new SelectList(_context.Sirketler, "Id", "Ad", fatura.SirketId);
            return View(fatura);
        }

        // --- 4. FATURA SİLME ---
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var fatura = await _context.Faturalar
                .Include(f => f.Sirket)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fatura == null) return NotFound();

            return View(fatura);
        }

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

        // --- 22. GÜN: EXCEL'E AKTARMA (ClosedXML) ---
        public async Task<IActionResult> ExcelaAktar()
        {
            var faturalar = await _context.Faturalar.Include(f => f.Sirket).ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Faturalar");

                // Başlıklar
                worksheet.Cell(1, 1).Value = "Fatura ID";
                worksheet.Cell(1, 2).Value = "Şirket Adı";
                worksheet.Cell(1, 3).Value = "Tarih";
                worksheet.Cell(1, 4).Value = "Tutar (TL)";
                worksheet.Cell(1, 5).Value = "Durum";

                // Veriler
                int currentRow = 2;
                foreach (var fatura in faturalar)
                {
                    worksheet.Cell(currentRow, 1).Value = fatura.Id;
                    worksheet.Cell(currentRow, 2).Value = fatura.Sirket?.Ad;
                    worksheet.Cell(currentRow, 3).Value = fatura.Tarih.ToString("dd.MM.yyyy");
                    worksheet.Cell(currentRow, 4).Value = fatura.Tutar;
                    worksheet.Cell(currentRow, 5).Value = fatura.Durum ? "Ödendi" : "Ödenmedi";
                    currentRow++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Faturalar.xlsx");
                }
            }
        }

        // --- 23. GÜN: PDF'E AKTARMA (QuestPDF) ---
        [HttpGet]
        public async Task<IActionResult> FaturaPdfIndir(int id)
        {
            var fatura = await _context.Faturalar
                .Include(f => f.Sirket)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fatura == null)
            {
                return NotFound();
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("FATURA DÖKÜMÜ")
                        .SemiBold().FontSize(24).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(x =>
                    {
                        x.Spacing(15);

                        x.Item().Text($"Şirket Adı: {fatura.Sirket?.Ad}").FontSize(14).SemiBold();
                        x.Item().LineHorizontal(1f);

                        x.Item().Text($"Tarih: {fatura.Tarih.ToString("dd.MM.yyyy")}");
                        x.Item().Text($"Tutar: {fatura.Tutar:N2} TL").FontColor(Colors.Green.Darken2).SemiBold();

                        string odemeDurumu = fatura.Durum ? "Ödendi" : "Ödenmedi";
                        string durumRengi = fatura.Durum ? Colors.Green.Medium : Colors.Red.Medium;

                        x.Item().Text($"Ödeme Durumu: {odemeDurumu}").FontColor(durumRengi).Bold();
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Fatura Masraf Sistemi - Sayfa ");
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Fatura_Detay_{id}.pdf");
        }
    }
}
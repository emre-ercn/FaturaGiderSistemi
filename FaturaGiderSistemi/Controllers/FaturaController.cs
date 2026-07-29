using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FaturaGiderSistemi.Data;
using FaturaGiderSistemi.Models; // Modelinin olduğu namespace (gerekirse burayı kendi projene göre ayarla)
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
            // Şirket tablosunu dahil ederek (Include) faturaları çekiyoruz ki view'da şirket adı gözüksün
            var faturalar = _context.Faturalar.Include(f => f.Sirket).AsQueryable();

            // 1. Arama Kelimesine Göre Filtreleme
            if (!string.IsNullOrEmpty(aramaKelimesi))
            {
                faturalar = faturalar.Where(f => f.FaturaNo.Contains(aramaKelimesi));
            }

            // 2. Duruma Göre Filtreleme (1: Ödenen, 0: Bekleyen)
            if (!string.IsNullOrEmpty(durumFiltresi))
            {
                bool odendiMi = durumFiltresi == "1";
                faturalar = faturalar.Where(f => f.Durum == odendiMi);
            }

            // Seçilen değerler formda kalmaya devam etsin diye ViewBag'e aktarıyoruz
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
    }
}
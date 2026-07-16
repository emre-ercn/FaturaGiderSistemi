using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FaturaGiderSistemi.Data;
using FaturaGiderSistemi.Models;

namespace FaturaGiderSistemi.Controllers
{
    public class FaturaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaturaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Fatura
        public async Task<IActionResult> Index(string aramaKelimesi)
        {
            ViewData["CurrentFilter"] = aramaKelimesi;
            var faturalar = from f in _context.Faturalar.Include(f => f.Kullanici).Include(f => f.Sirket)
                            select f;

            if (!String.IsNullOrEmpty(aramaKelimesi))
            {
                faturalar = faturalar.Where(s => s.FaturaNo.Contains(aramaKelimesi));
            }

            return View(await faturalar.ToListAsync());
        }

        // GET: Fatura/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var fatura = await _context.Faturalar
                .Include(f => f.Kullanici)
                .Include(f => f.Sirket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fatura == null) return NotFound();
            return View(fatura);
        }

        // GET: Fatura/Create
        public IActionResult Create()
        {
            ViewBag.SirketId = new SelectList(_context.Sirketler, "Id", "Ad");
            ViewBag.KullaniciId = new SelectList(_context.Kullanicilar, "Id", "Ad");
            return View();
        }

        // POST: Fatura/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FaturaNo,FisNo,Tutar,KdvOrani,ToplamTutar,Tarih,Durum,SirketId,KullaniciId")] Fatura fatura)
        {
            ModelState.Remove("Sirket");
            ModelState.Remove("Kullanici");

            if (ModelState.IsValid)
            {
                _context.Add(fatura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SirketId = new SelectList(_context.Sirketler, "Id", "Ad", fatura.SirketId);
            ViewBag.KullaniciId = new SelectList(_context.Kullanicilar, "Id", "Ad", fatura.KullaniciId);
            return View(fatura);
        }

        // GET: Fatura/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var fatura = await _context.Faturalar.FindAsync(id);
            if (fatura == null) return NotFound();
            ViewBag.SirketId = new SelectList(_context.Sirketler, "Id", "Ad", fatura.SirketId);
            ViewBag.KullaniciId = new SelectList(_context.Kullanicilar, "Id", "Ad", fatura.KullaniciId);
            return View(fatura);
        }

        // POST: Fatura/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FaturaNo,FisNo,Tutar,KdvOrani,ToplamTutar,Tarih,Durum,SirketId,KullaniciId")] Fatura fatura)
        {
            if (id != fatura.Id) return NotFound();

            ModelState.Remove("Sirket");
            ModelState.Remove("Kullanici");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fatura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaturaExists(fatura.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SirketId = new SelectList(_context.Sirketler, "Id", "Ad", fatura.SirketId);
            ViewBag.KullaniciId = new SelectList(_context.Kullanicilar, "Id", "Ad", fatura.KullaniciId);
            return View(fatura);
        }

        // GET: Fatura/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var fatura = await _context.Faturalar
                .Include(f => f.Kullanici)
                .Include(f => f.Sirket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fatura == null) return NotFound();
            return View(fatura);
        }

        // POST: Fatura/Delete/5
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

        private bool FaturaExists(int id)
        {
            return _context.Faturalar.Any(e => e.Id == id);
        }
    }
}
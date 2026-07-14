using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Faturalar.Include(f => f.Kullanici).Include(f => f.Sirket);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Fatura/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fatura = await _context.Faturalar
                .Include(f => f.Kullanici)
                .Include(f => f.Sirket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fatura == null)
            {
                return NotFound();
            }

            return View(fatura);
        }

        // GET: Fatura/Create
        public IActionResult Create()
        {
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilar, "Id", "Id");
            ViewData["SirketId"] = new SelectList(_context.Sirketler, "Id", "Id");
            return View();
        }

        // POST: Fatura/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FisNo,Tarih,ToplamTutar,KdvOrani,Durum,SirketId,KullaniciId")] Fatura fatura)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fatura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilar, "Id", "Id", fatura.KullaniciId);
            ViewData["SirketId"] = new SelectList(_context.Sirketler, "Id", "Id", fatura.SirketId);
            return View(fatura);
        }

        // GET: Fatura/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fatura = await _context.Faturalar.FindAsync(id);
            if (fatura == null)
            {
                return NotFound();
            }
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilar, "Id", "Id", fatura.KullaniciId);
            ViewData["SirketId"] = new SelectList(_context.Sirketler, "Id", "Id", fatura.SirketId);
            return View(fatura);
        }

        // POST: Fatura/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FisNo,Tarih,ToplamTutar,KdvOrani,Durum,SirketId,KullaniciId")] Fatura fatura)
        {
            if (id != fatura.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fatura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaturaExists(fatura.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilar, "Id", "Id", fatura.KullaniciId);
            ViewData["SirketId"] = new SelectList(_context.Sirketler, "Id", "Id", fatura.SirketId);
            return View(fatura);
        }

        // GET: Fatura/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fatura = await _context.Faturalar
                .Include(f => f.Kullanici)
                .Include(f => f.Sirket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fatura == null)
            {
                return NotFound();
            }

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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FaturaExists(int id)
        {
            return _context.Faturalar.Any(e => e.Id == id);
        }
    }
}

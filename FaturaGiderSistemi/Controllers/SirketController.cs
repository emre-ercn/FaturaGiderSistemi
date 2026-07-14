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
    public class SirketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SirketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sirket
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sirketler.ToListAsync());
        }

        // GET: Sirket/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sirket = await _context.Sirketler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sirket == null)
            {
                return NotFound();
            }

            return View(sirket);
        }

        // GET: Sirket/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sirket/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ad,VergiNo")] Sirket sirket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sirket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sirket);
        }

        // GET: Sirket/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sirket = await _context.Sirketler.FindAsync(id);
            if (sirket == null)
            {
                return NotFound();
            }
            return View(sirket);
        }

        // POST: Sirket/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,VergiNo")] Sirket sirket)
        {
            if (id != sirket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sirket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SirketExists(sirket.Id))
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
            return View(sirket);
        }

        // GET: Sirket/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sirket = await _context.Sirketler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sirket == null)
            {
                return NotFound();
            }

            return View(sirket);
        }

        // POST: Sirket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sirket = await _context.Sirketler.FindAsync(id);
            if (sirket != null)
            {
                _context.Sirketler.Remove(sirket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SirketExists(int id)
        {
            return _context.Sirketler.Any(e => e.Id == id);
        }
    }
}

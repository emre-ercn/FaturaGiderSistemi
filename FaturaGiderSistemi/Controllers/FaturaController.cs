using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FaturaGiderSistemi.Data;
using FaturaGiderSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace FaturaGiderSistemi.Controllers
{
    [Authorize]
    public class FaturaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaturaController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public IActionResult Index()
        {
            var faturalar = _context.Faturalar
                                    .Include(f => f.Sirket)
                                    .Include(f => f.Kullanici)
                                    .ToList();
            return View(faturalar);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Sirketler = new SelectList(_context.Sirketler, "Id", "Ad");
            ViewBag.Kullanicilar = new SelectList(_context.Kullanicilar, "Id", "Ad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Fatura fatura)
        {
           
            ModelState.Remove("Sirket");
            ModelState.Remove("Kullanici");

            if (ModelState.IsValid)
            {
                _context.Faturalar.Add(fatura);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Sirketler = new SelectList(_context.Sirketler, "Id", "Ad", fatura.SirketId);
            ViewBag.Kullanicilar = new SelectList(_context.Kullanicilar, "Id", "Ad", fatura.KullaniciId);
            return View(fatura);
        }
    }
}
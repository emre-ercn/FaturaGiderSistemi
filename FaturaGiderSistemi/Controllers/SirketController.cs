using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            return View(await _context.Sirketler.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ad,VergiNo,Adres")] Sirket sirket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sirket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sirket);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FaturaGiderSistemi.Data;
using System.Linq;

namespace FaturaGiderSistemi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Dashboard Kartlarý için Deðerler
            ViewBag.ToplamFaturaSayisi = _context.Faturalar.Count();

            ViewBag.ToplamSirketSayisi = _context.Sirketler.Count();

            ViewBag.ToplamOdenen = _context.Faturalar
                .Where(f => f.Durum == true)
                .Sum(f => (decimal?)f.ToplamTutar) ?? 0;

            ViewBag.ToplamBekleyen = _context.Faturalar
                .Where(f => f.Durum == false)
                .Sum(f => (decimal?)f.ToplamTutar) ?? 0;

            return View();
        }
    }
}
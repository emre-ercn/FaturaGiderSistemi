using FaturaGiderSistemi.Data;
using FaturaGiderSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            // Senin Index.cshtml dosyanýn beklediði deðiþken isimleri:
            ViewBag.ToplamFaturaSayisi = _context.Faturalar.Count();
            ViewBag.ToplamSirketSayisi = _context.Sirketler.Count();

            // Veritabanýnda fatura durumu nasýl tutuluyorsa (örneðin boolean true/false) 
            ViewBag.ToplamOdenen = _context.Faturalar.Where(x => x.Durum == true).Sum(y => (decimal?)y.Tutar) ?? 0;
            ViewBag.ToplamBekleyen = _context.Faturalar.Where(x => x.Durum == false).Sum(y => (decimal?)y.Tutar) ?? 0;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
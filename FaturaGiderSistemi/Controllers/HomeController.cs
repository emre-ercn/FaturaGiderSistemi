using FaturaGiderSistemi.Data;
using FaturaGiderSistemi.Models; // Kendi Model klasörümüz
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FaturaGiderSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Veritabaný baðlantýsýný ana sayfaya dahil ediyoruz
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Ýstatistikleri Veritabanýndan Hesaplýyoruz
            var toplamFatura = await _context.Faturalar.CountAsync();
            var toplamOdendi = await _context.Faturalar.Where(f => f.Durum == true).SumAsync(f => f.Tutar);
            var toplamOdenmedi = await _context.Faturalar.Where(f => f.Durum == false).SumAsync(f => f.Tutar);

            // 2. Hesaplanan verileri arayüze (View) taþýyoruz
            ViewBag.ToplamFatura = toplamFatura;
            ViewBag.ToplamOdendi = toplamOdendi;
            ViewBag.ToplamOdenmedi = toplamOdenmedi;

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
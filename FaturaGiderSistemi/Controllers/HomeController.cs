using Microsoft.AspNetCore.Mvc;
using FaturaGiderSistemi.Data; // Kendi Data klasörünün yoluna göre gerekirse düzelt
using System.Linq;

namespace FaturaGiderSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Veritabanýný (Context) HomeController'a enjekte ediyoruz
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 1. Toplam kayýtlý fatura sayýsý
            ViewBag.ToplamFaturaSayisi = _context.Faturalar.Count();

            // 2. Toplam kayýtlý þirket sayýsý
            ViewBag.ToplamSirketSayisi = _context.Sirketler.Count();

            // 3. Durumu "Ödendi" (True) olan faturalarýn toplam tutarý
            // Veritabaný boþken hata vermemesi için (decimal?) ve null coalescing (?? 0) kullanýyoruz
            ViewBag.ToplamOdenen = _context.Faturalar
                .Where(f => f.Durum == true)
                .Sum(f => (decimal?)f.ToplamTutar) ?? 0;

            // 4. Durumu "Bekliyor" (False) olan faturalarýn toplam tutarý
            ViewBag.ToplamBekleyen = _context.Faturalar
                .Where(f => f.Durum == false)
                .Sum(f => (decimal?)f.ToplamTutar) ?? 0;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
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
            var toplamSirket = await _context.Sirketler.CountAsync(); // Yeni eklendi: Þirket sayýsý

            var toplamOdendi = await _context.Faturalar.Where(f => f.Durum == true).SumAsync(f => f.Tutar);
            var toplamOdenmedi = await _context.Faturalar.Where(f => f.Durum == false).SumAsync(f => f.Tutar);

            // GRAFÝKLER ÝÇÝN YENÝ EKLENEN KISIM: Adetleri buluyoruz
            var odenenAdet = await _context.Faturalar.CountAsync(f => f.Durum == true);
            var bekleyenAdet = await _context.Faturalar.CountAsync(f => f.Durum == false);

            // YENÝ: Anasayfadaki tablo için Son 5 Ýþlemi çekiyoruz
            var sonIslemler = await _context.Faturalar
                .Include(f => f.Sirket)
                .OrderByDescending(f => f.Id) // En son eklenen en üstte gelsin
                .Take(5) // Sadece 5 tane alalým
                .ToListAsync();

            // 2. Hesaplanan verileri arayüze (View) taþýyoruz
            ViewBag.ToplamFatura = toplamFatura;
            ViewBag.ToplamSirket = toplamSirket;
            ViewBag.ToplamOdendi = toplamOdendi;
            ViewBag.ToplamOdenmedi = toplamOdenmedi;
            ViewBag.OdenenAdet = odenenAdet;
            ViewBag.BekleyenAdet = bekleyenAdet;

            // Son 5 faturayý modele gönderiyoruz
            return View(sonIslemler);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // 30. GÜN EKLENTÝSÝ: 404 sayfasýný yakalayacak güncellenmiþ Error metodu
        [Route("Home/Error/{statusCode}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode == 404)
            {
                // 404 Hatasý için özel görünüm
                return View("NotFound");
            }

            // Diðer genel hatalar için
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
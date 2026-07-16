using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FaturaGiderSistemi.Data;

namespace FaturaGiderSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Veritabaný baðlantýsýný buraya da çekiyoruz ki hesaplama yapabilelim
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Veritabanýndaki kayýtlarý sayýp, özetleri ViewBag ile arayüze gönderiyoruz
            ViewBag.ToplamSirket = _context.Sirketler.Count();
            ViewBag.ToplamFatura = _context.Faturalar.Count();

            // Durumu true (Ödendi) olanlarýn toplam tutarý
            ViewBag.OdenenTutar = _context.Faturalar.Where(f => f.Durum).Sum(f => f.ToplamTutar);

            // Durumu false (Ödenmedi) olanlarýn toplam tutarý
            ViewBag.BekleyenTutar = _context.Faturalar.Where(f => f.Durum == false).Sum(f => f.ToplamTutar);

            return View();
        }
    }
}
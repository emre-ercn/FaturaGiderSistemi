using FaturaGiderSistemi.Data;
using FaturaGiderSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace FaturaGiderSistemi.Controllers
{
    public class FaturaController : Controller
    {
        // KANKA DİKKAT: ApplicationDbContext yazan yeri kendi DbContext adınla değiştir
        private readonly ApplicationDbContext _context;

        public FaturaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fatura Listeleme Sayfası
        public IActionResult Index()
        {
            var faturalar = _context.Faturalar.ToList();
            return View(faturalar);
        }

        // Fatura Ekleme Sayfasını Açma (GET)
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.SirketlerListesi = new SelectList(_context.Sirketler.ToList(), "Id", "Ad");
            return View();
        }

        // Fatura Ekleme İşlemi (POST)
        [HttpPost]
        public IActionResult Create(Fatura fatura)
        {
            // Validasyon hatalarını yoksayıyoruz
            ModelState.Remove("Sirket");
            ModelState.Remove("Kullanici");

            // Veritabanında hiç kullanıcı var mı kontrol et
            var ilkKullanici = _context.Kullanicilar.FirstOrDefault();

            // Eğer tablo boşsa, veritabanına otomatik olarak seni ekliyoruz
            if (ilkKullanici == null)
            {
                ilkKullanici = new Kullanici
                {
                    Ad = "Emre",
                    Soyad = "Ercan",
                    Email = "emre@test.com",
                    Sifre = "123456",
                    Rol = "Admin"
                };
                _context.Kullanicilar.Add(ilkKullanici);
                _context.SaveChanges(); // SQL yeni kullanıcıya ID versin diye önce bunu kaydediyoruz
            }

            // Artık sistemdeki kullanıcının ID'sini faturaya güvenle atıyoruz
            fatura.KullaniciId = ilkKullanici.Id;

            // Faturayı kaydet
            if (ModelState.IsValid)
            {
                _context.Faturalar.Add(fatura);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Eğer modelde başka bir hata varsa, formu tekrar göster
            ViewBag.SirketlerListesi = new SelectList(_context.Sirketler.ToList(), "Id", "Ad");
            return View(fatura);
        }
    }
}
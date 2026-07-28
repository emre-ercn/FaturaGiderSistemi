using FaturaGiderSistemi.Data;
using FaturaGiderSistemi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FaturaGiderSistemi.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KullaniciController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 17. GÜN: GİRİŞ YAP (LOGIN) VE ÇIKIŞ YAP
        // ==========================================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Kullanici p)
        {
            // Kullanıcı adı ve şifre kontrolü
            var bilgiler = _context.Kullanicilar.FirstOrDefault(x => x.Ad == p.Ad && x.Sifre == p.Sifre);

            if (bilgiler != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, p.Ad)
                };

                var useridentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);

                await HttpContext.SignInAsync(principal);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Hata = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Kullanici");
        }

        // ==========================================
        // MEVCUT CRUD (EKLE, SİL, GÜNCELLE, LİSTELE) İŞLEMLERİ
        // ==========================================

        public IActionResult Index()
        {
            var degerler = _context.Kullanicilar.ToList();
            return View(degerler);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Kullanici p)
        {
            if (ModelState.IsValid)
            {
                _context.Kullanicilar.Add(p);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(p);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var kullanici = _context.Kullanicilar.Find(id);
            if (kullanici == null)
            {
                return NotFound();
            }
            return View(kullanici);
        }

        [HttpPost]
        public IActionResult Edit(Kullanici p)
        {
            if (ModelState.IsValid)
            {
                _context.Kullanicilar.Update(p);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(p);
        }

        public IActionResult Delete(int id)
        {
            var kullanici = _context.Kullanicilar.Find(id);
            if (kullanici != null)
            {
                _context.Kullanicilar.Remove(kullanici);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
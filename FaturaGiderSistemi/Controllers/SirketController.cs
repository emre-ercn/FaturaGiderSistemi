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

        // Index metoduna arama kelimesini yakalaması için "searchString" parametresi ekliyoruz
        public async Task<IActionResult> Index(string searchString)
        {
            // 1. Veritabanındaki tüm şirketleri sorgulanabilir halde alıyoruz
            var sirketler = from s in _context.Sirketler select s;

            // 2. Eğer kullanıcı arama kutusuna bir şey yazmışsa (boş değilse) filtreleme yapıyoruz
            if (!String.IsNullOrEmpty(searchString))
            {
                sirketler = sirketler.Where(s => s.Ad.Contains(searchString));
            }

            // 3. Filtrelenmiş (veya arama yapılmamışsa tüm) listeyi sayfaya gönderiyoruz
            return View(await sirketler.ToListAsync());
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
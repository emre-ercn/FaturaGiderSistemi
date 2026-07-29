using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaturaGiderSistemi.Data;
using FaturaGiderSistemi.Models; // Model namespace'in farklıysa burayı düzelt kanka
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FaturaGiderSistemi.Controllers
{
    [Authorize] // Sayfaya sadece giriş yapanlar girebilsin
    public class RaporlarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RaporlarController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult>Index()
        {
            // Şirket adlarını da alabilmek için Include yapıyoruz
            var faturalar = await _context.Faturalar.Include(f => f.Sirket).ToListAsync();

            // Verileri hesaplayıp ViewModel'e dolduruyoruz
            var model = new RaporViewModel
            {
                ToplamOdenen = faturalar.Where(f => f.Durum == true).Sum(f => f.ToplamTutar),
                ToplamBekleyen = faturalar.Where(f => f.Durum == false).Sum(f => f.ToplamTutar),
                ToplamFaturaSayisi = faturalar.Count,

                // Şirketlere göre gruplama (GroupBy) ve toplam alma işlemleri
                SirketRaporlari = faturalar
                    .Where(f => f.Sirket != null)
                    .GroupBy(f => f.Sirket.Ad)
                    .Select(g => new SirketRaporu
                    {
                        SirketAdi = g.Key, // Şirket adı
                        FaturaSayisi = g.Count(), // O şirkete ait fatura adedi
                        ToplamTutar = g.Sum(f => f.ToplamTutar) // O şirketin toplam hacmi
                    })
                    .OrderByDescending(r => r.ToplamTutar) // En çok ciro yapılanı en üste koyar
                    .ToList()
            };

            return View(model);
        }
    }
}
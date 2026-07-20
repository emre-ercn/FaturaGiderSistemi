using ClosedXML.Excel;
using System.IO;
using FaturaGiderSistemi.Data;
using FaturaGiderSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FaturaGiderSistemi.Controllers
{
    public class FaturaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaturaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fatura Listeleme Sayfası
        public IActionResult Index(string aramaKelimesi, int? pageNumber)
        {
            ViewData["ArananKelime"] = aramaKelimesi;
            var faturalar = _context.Faturalar.Include(f => f.Sirket).AsQueryable();

            if (!string.IsNullOrEmpty(aramaKelimesi))
            {
                faturalar = faturalar.Where(f => f.FaturaNo.Contains(aramaKelimesi) ||
                                                 f.Sirket.Ad.Contains(aramaKelimesi));
            }

            int pageSize = 5;
            int pageIndex = pageNumber ?? 1;

            var count = faturalar.Count();
            var items = faturalar.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            var paginatedList = new PaginatedList<Fatura>(items, count, pageIndex, pageSize);

            return View(paginatedList);
        }
        public IActionResult ExportToExcel()
        {
            // Veritabanından faturaları şirket bilgisiyle beraber çekiyoruz (Include çok önemli)
            var faturalar = _context.Faturalar.Include(f => f.Sirket).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Faturalar");

                // Başlık Satırı (1. Satır)
                worksheet.Cell(1, 1).Value = "Fatura No";
                worksheet.Cell(1, 2).Value = "Fiş No";
                worksheet.Cell(1, 3).Value = "Tarih";
                worksheet.Cell(1, 4).Value = "Şirket Adı";
                worksheet.Cell(1, 5).Value = "Tutar";
                worksheet.Cell(1, 6).Value = "KDV Oranı (%)";
                worksheet.Cell(1, 7).Value = "Toplam Tutar";
                worksheet.Cell(1, 8).Value = "Durum";

                // Başlıkları biraz havalı yapalım (Kalın ve arka planı gri)
                var headerRange = worksheet.Range(1, 1, 1, 8);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Verileri veritabanından alıp satır satır dolduruyoruz (2. satırdan başlıyoruz)
                int row = 2;
                foreach (var fatura in faturalar)
                {
                    worksheet.Cell(row, 1).Value = fatura.FaturaNo;
                    worksheet.Cell(row, 2).Value = fatura.FisNo;
                    worksheet.Cell(row, 3).Value = fatura.Tarih.ToString("dd.MM.yyyy"); // Tarihi düzgün gösterelim
                    worksheet.Cell(row, 4).Value = fatura.Sirket?.Ad; // Şirket adı (İlişkisel Tablo)
                    worksheet.Cell(row, 5).Value = fatura.Tutar;
                    worksheet.Cell(row, 6).Value = fatura.KdvOrani;
                    worksheet.Cell(row, 7).Value = fatura.ToplamTutar;

                    // Durum true/false ise Excel'de şık dursun diye metne çeviriyoruz
                    worksheet.Cell(row, 8).Value = fatura.Durum ? "Ödendi" : "Bekliyor";

                    row++;
                }

                // Sütun genişliklerini içindeki metne göre otomatik ayarla (jilet gibi görünmesi için)
                worksheet.Columns().AdjustToContents();

                // Dosyayı hafızada oluşturup kullanıcıya "Faturalar.xlsx" adıyla indirttiriyoruz
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Faturalar.xlsx");
                }
            }
        }

        // Fatura Ekleme Sayfasını Açma (GET) - 405 Hatasını Çözen Kısım
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
            ModelState.Remove("Sirket");
            ModelState.Remove("Kullanici");

            var ilkKullanici = _context.Kullanicilar.FirstOrDefault();

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
                _context.SaveChanges();
            }

            fatura.KullaniciId = ilkKullanici.Id;

            if (ModelState.IsValid)
            {
                _context.Faturalar.Add(fatura);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SirketlerListesi = new SelectList(_context.Sirketler.ToList(), "Id", "Ad");
            return View(fatura);
        }

        // Excel Çıktısı Al Butonu İçin (Şimdilik hata vermemesi için boş geçiyoruz)
        [HttpGet]
        public IActionResult ExcelCiktisiAl()
        {
            // Excel için de veritabanından çekerken Şirketleri dahil etmemiz gerekiyor (Include ekledik)
            var faturalar = _context.Faturalar.Include(f => f.Sirket).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Fatura Listesi");

                // Başlıkları ekliyoruz (1. Satır) - Şirket Adı'nı 3. sütun olarak araya ekledik
                worksheet.Cell(1, 1).Value = "Fatura No";
                worksheet.Cell(1, 2).Value = "Tarih";
                worksheet.Cell(1, 3).Value = "Şirket Adı";
                worksheet.Cell(1, 4).Value = "KDV Oranı (%)";
                worksheet.Cell(1, 5).Value = "Toplam Tutar (₺)";
                worksheet.Cell(1, 6).Value = "Durum";

                // Başlıkları kalın (bold) yapıyoruz (Artık F sütununa kadar gidiyor)
                worksheet.Range("A1:F1").Style.Font.Bold = true;

                // Verileri satır satır yazdırıyoruz (2. Satırdan itibaren)
                int currentRow = 2;
                foreach (var fatura in faturalar)
                {
                    worksheet.Cell(currentRow, 1).Value = fatura.FaturaNo;
                    worksheet.Cell(currentRow, 2).Value = fatura.Tarih.ToString("dd.MM.yyyy");
                    // Şirket boşsa hata vermesin diye tire (-) koyduruyoruz
                    worksheet.Cell(currentRow, 3).Value = fatura.Sirket != null ? fatura.Sirket.Ad : "-";
                    worksheet.Cell(currentRow, 4).Value = fatura.KdvOrani;
                    worksheet.Cell(currentRow, 5).Value = fatura.ToplamTutar;
                    worksheet.Cell(currentRow, 6).Value = fatura.Durum ? "Ödendi" : "Bekliyor";

                    currentRow++;
                }

                // Sütun genişliklerini içeriğe göre otomatik ayarla
                worksheet.Columns().AdjustToContents();

                // Dosyayı RAM'de oluşturup kullanıcıya indirtiyoruz
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Faturalar.xlsx");
                }
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var fatura = _context.Faturalar.Find(id);
            if (fatura == null)
            {
                return NotFound();
            }

            
            ViewBag.SirketlerListesi = new SelectList(_context.Sirketler.ToList(), "Id", "Ad", fatura.SirketId);

            return View(fatura);
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var fatura = _context.Faturalar.Find(id);
            if (fatura == null) return NotFound();

            return View(fatura);
        }
  
        [HttpPost]
        public IActionResult Edit(Fatura fatura)
        {
            ModelState.Remove("Sirket");
            ModelState.Remove("Kullanici");

            if (ModelState.IsValid)
            {
               
                var mevcutFatura = _context.Faturalar.Find(fatura.Id);

                if (mevcutFatura != null)
                {
                   
                    mevcutFatura.FaturaNo = fatura.FaturaNo;
                    mevcutFatura.Tarih = fatura.Tarih;
                    mevcutFatura.KdvOrani = fatura.KdvOrani;
                    mevcutFatura.ToplamTutar = fatura.ToplamTutar;
                    mevcutFatura.Durum = fatura.Durum;

                   
         
                    if (fatura.SirketId != 0)
                    {
                        mevcutFatura.SirketId = fatura.SirketId;
                    }

                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            
            ViewBag.SirketlerListesi = new SelectList(_context.Sirketler.ToList(), "Id", "Ad", fatura.SirketId);
            return View(fatura);
        }

        
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var fatura = _context.Faturalar.Find(id);
            if (fatura != null)
            {
                _context.Faturalar.Remove(fatura);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }

    }
# 📊 Fatura ve Masraf Yönetim Sistemi (Staj Projesi)

Bu proje, 30 günlük staj programım kapsamında geliştirilmiş bir Fatura ve Masraf Takip otomasyonudur. Sistemin temel amacı, işletmelerin fatura ve harcama süreçlerini dijitalleştirerek daha düzenli, hızlı ve takip edilebilir bir yapıya kavuşturmaktır.

## 🎯 Projenin Amacı ve Kazandırdıkları
Staj süresince geliştirdiğim bu projede, bir yazılımın sıfırdan tasarlanıp kodlanması süreçlerini deneyimledim. Veritabanı işlemleri, kullanıcı arayüzü tasarımı ve raporlama gibi gerçek hayat senaryoları üzerinde çalıştım.

## ⚙️ Kullanılan Teknolojiler
* **Programlama Dili:** C# 
* **Veritabanı:** MSSQL 
* **Ekstra Kütüphaneler:** ClosedXML (Excel çıktıları ve raporlama için) vb.
* **Geliştirme Ortamı:** Visual Studio 

## 🚀 Temel Özellikler
* **Fatura Yönetimi:** Yeni fatura ekleme, düzenleme ve silme işlemleri.
* **Masraf Takibi:** Şirket içi masrafların kategorilendirilip sisteme girilmesi.
* **Raporlama:** Girilen verilerin Excel formatında dışa aktarılması (ClosedXML ile).
* **Kullanıcı Dostu Arayüz:** İşlemlerin hızlıca yapılabildiği sade tasarım.

## 📸 Ekran Görüntüleri

Projenin arayüzünden bazı görseller:

<img width="1881" height="876" alt="gün_1_1" src="https://github.com/user-attachments/assets/157973b5-2337-4363-b61c-261e6c574573" />

<img width="945" height="824" alt="gün_5_1" src="https://github.com/user-attachments/assets/cb4b65c8-e68e-4941-a3da-0d2f06447000" />
<img width="1843" height="659" alt="gün_7_1" src="https://github.com/user-attachments/assets/10b2f7a6-1e13-41c5-b689-2a1f7d323559" />

<img width="1919" height="653" alt="gün_10_1" src="https://github.com/user-attachments/assets/0d8e7c64-7854-436c-af73-bc208d1e7517" />
<img width="1920" height="415" alt="gün_12_1" src="https://github.com/user-attachments/assets/463741e3-2fa5-4dec-9911-65f7b1d1140f" />
<img width="1920" height="880" alt="gün_13_2" src="https://github.com/user-attachments/assets/8a12c201-d6d7-49a0-991c-edaef66d3df4" />
<img width="1920" height="729" alt="gün_16_1" src="https://github.com/user-attachments/assets/dea3b896-f19d-4e18-94ef-f427ed430677" />
<img width="1920" height="623" alt="gün_18_2" src="https://github.com/user-attachments/assets/fcde23ff-94b6-4051-be0e-4e4609a62dbb" />

## 💻 Kod Yapısından Bir Örnek

Projemi geliştirirken yazdığım ve en çok uğraştığım/önemli gördüğüm kod bloklarından birisi:

// ClosedXML kütüphanesi kullanılarak veritabanındaki faturaların Excel formatında dışa aktarılması
[HttpGet]
public IActionResult FaturalariExcelIndir()
{
    using (var workbook = new XLWorkbook())
    {
        var worksheet = workbook.Worksheets.Add("Fatura Listesi");
        
        // Excel başlık satırlarının oluşturulması
        worksheet.Cell(1, 1).Value = "Fatura Numarası";
        worksheet.Cell(1, 2).Value = "Tarih";
        worksheet.Cell(1, 3).Value = "Toplam Tutar";

        // Veritabanından gelen verilerin döngü ile satırlara işlenmesi
        // (Burada veritabanı listeleme kodların yer alır)

        // Dosyanın belleğe alınıp kullanıcıya indirtilmesi
        using (var stream = new MemoryStream())
        {
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FaturaRaporu.xlsx");
        }
    }
}
<img width="1242" height="676" alt="Ekran Alıntısı" src="https://github.com/user-attachments/assets/80368764-0087-4d71-858a-2acb76794b53" />

using System;

namespace FaturaGiderSistemi.Models
{
    public class Fatura
    {
        public int Id { get; set; }
        public string FisNo { get; set; }
        public DateTime Tarih { get; set; }
        public decimal ToplamTutar { get; set; }
        public decimal KdvOrani { get; set; }
        public string Durum { get; set; } // Örn: "Bekliyor", "Onaylandi"

        // İlişkili Tablo Bağlantıları
        public int SirketId { get; set; }
        public Sirket Sirket { get; set; }

        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }
    }
}
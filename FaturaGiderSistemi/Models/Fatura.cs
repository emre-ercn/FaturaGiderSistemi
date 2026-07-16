using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaturaGiderSistemi.Models
{
    public class Fatura
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Fatura numarası zorunludur.")]
        [Display(Name = "Fatura Numarası")]
        public string FaturaNo { get; set; }

        [Display(Name = "Fiş Numarası")]
        public string FisNo { get; set; }

        public decimal Tutar { get; set; }

        [Display(Name = "KDV Oranı")]
        public int KdvOrani { get; set; }

        [Display(Name = "Toplam Tutar")]
        public decimal ToplamTutar { get; set; }

        public DateTime Tarih { get; set; }

        [Display(Name = "Fatura Durumu")]
        public bool Durum { get; set; }

        // --- TABLO BAĞLANTILARI (FOREIGN KEYS) ---

        [Display(Name = "Şirket")]
        public int SirketId { get; set; }
        [ForeignKey("SirketId")]
        public Sirket Sirket { get; set; }

        [Display(Name = "Sisteme Giren Kullanıcı")]
        public int KullaniciId { get; set; }
        [ForeignKey("KullaniciId")]
        public Kullanici Kullanici { get; set; }
    }
}
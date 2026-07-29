using System.Collections.Generic;

namespace FaturaGiderSistemi.Models
{
    public class RaporViewModel
    {
        public decimal ToplamOdenen { get; set; }
        public decimal ToplamBekleyen { get; set; }
        public int ToplamFaturaSayisi { get; set; }

        // Çakışmayı çözen doğru ve eksiksiz liste tanımı:
        public System.Collections.Generic.List<SirketRaporu> SirketRaporlari { get; set; }
    }

    public class SirketRaporu
    {
        public string SirketAdi { get; set; }
        public int FaturaSayisi { get; set; }
        public decimal ToplamTutar { get; set; }
    }
}
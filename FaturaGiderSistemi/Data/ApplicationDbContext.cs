using Microsoft.EntityFrameworkCore;
using FaturaGiderSistemi.Models;

namespace FaturaGiderSistemi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Veritabanında oluşacak tablolarımızın isimleri
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Sirket> Sirketler { get; set; }
        public DbSet<Fatura> Faturalar { get; set; }
    }
}
using FaturaGiderSistemi.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritabanı Bağlantısı (Kendi ConnectionString'in appsettings.json'da olmalı)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 17. GÜN: Güvenlik ve Çerez Ayarları
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Kullanici/Login"; // Giriş yapmayanları buraya atacağız
        options.Cookie.Name = "FaturaSistemiAuth";
        options.ExpireTimeSpan = TimeSpan.FromHours(2); // 2 saat sonra otomatik çıkış
    });
// QuestPDF Topluluk Lisansı
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 30. GÜN EKLENTİSİ: 404 ve diğer durum kodları için özel sayfa yönlendirmesi
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// SIRA ÇOK ÖNEMLİ: Authentication, Authorization'dan ÖNCE gelmeli
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
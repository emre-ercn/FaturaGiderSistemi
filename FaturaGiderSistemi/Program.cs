using FaturaGiderSistemi.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritabaný Baðlantýsý (Kendi ConnectionString'in appsettings.json'da olmalý)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 17. GÜN: Güvenlik ve Çerez Ayarlarý
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Kullanici/Login"; // Giriþ yapmayanlarý buraya atacaðýz
        options.Cookie.Name = "FaturaSistemiAuth";
        options.ExpireTimeSpan = TimeSpan.FromHours(2); // 2 saat sonra otomatik çýkýþ
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// SIRA ÇOK ÖNEMLÝ: Authentication, Authorization'dan ÖNCE gelmeli
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
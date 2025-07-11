// Đường dẫn: Program.cs
using Microsoft.EntityFrameworkCore;
using Web_ThoiTrang.Data;
using Web_ThoiTrang.Models; // Cần thiết nếu dùng Identity (IdentityUser)
using Microsoft.AspNetCore.Identity; // Cần thiết nếu dùng Identity
using Microsoft.AspNetCore.Authentication.Cookies; // Cần thiết nếu dùng Cookie Authentication
using System.Security.Claims; // Cần thiết cho Claims
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Cấu hình Authentication ===
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Đường dẫn đến trang đăng nhập
        options.AccessDeniedPath = "/Home/AccessDenied"; // Đường dẫn khi bị từ chối truy cập (có thể tạo Controller/Action này sau)
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Thời gian sống của cookie
        options.SlidingExpiration = true; // Gia hạn thời gian sống nếu người dùng hoạt động
        // Đảm bảo cookie được gửi an toàn qua HTTPS
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Luôn gửi cookie qua HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax; // Hoặc SameSiteMode.Strict tùy theo yêu cầu bảo mật
    });

// === Cấu hình Authorization ===
builder.Services.AddAuthorization(options =>
{
    // Định nghĩa Policy cho Admin
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    // Định nghĩa Policy cho Customer
    options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Customer"));
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

// === Thêm các middleware authentication và authorization ===
app.UseAuthentication(); // Phải ở trước UseAuthorization
app.UseAuthorization();
// =======================================================

// Đăng ký Area Route (phải trước route mặc định)
app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
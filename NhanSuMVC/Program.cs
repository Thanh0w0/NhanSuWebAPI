using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddNotyf(config => { config.DurationInSeconds = 5; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });

builder.Services.AddDistributedMemoryCache(); // Cần thiết cho việc sử dụng session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; // Bảo mật cookie session
    options.Cookie.IsEssential = true; // Bắt buộc để session hoạt động
});


// Cấu hình Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/DangNhap/Index"; // Đường dẫn tới trang đăng nhập
        options.AccessDeniedPath = "/DangNhap/AccessDenied"; // Đường dẫn khi không có quyền truy cập
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Thời gian hết hạn của cookie
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Quản trị viên"));
    options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Nhân viên"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseNotyf();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DangNhap}/{action=Index}/{id?}");


// Route cho trang đăng nhập ở cấp gốc
app.MapControllerRoute(
    name: "login",
    pattern: "DangNhap/{action=Index}/{id?}",
    defaults: new { controller = "DangNhap" });

// Route cho các trang Admin
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "nhanvien",
    pattern: "NhanVien/{controller=Home}/{action=Index}/{id?}");

app.Run();

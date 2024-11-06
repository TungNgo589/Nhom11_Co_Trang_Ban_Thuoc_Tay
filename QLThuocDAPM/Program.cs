using Microsoft.EntityFrameworkCore;
using QLThuocDAPM.Data;
using QLThuocDAPM.Common;
using Common = QLThuocDAPM.Common.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<Common>();
// Add services to the container.
builder.Services.AddControllersWithViews();

// Add database context
builder.Services.AddDbContext<QlthuocDapm4Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Hshop"));
});

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // Timeout after 10 minutes
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Cấu hình routing hỗ trợ Areas
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession(); // Sử dụng session trước khi dùng authentication và authorization
app.UseRouting();

app.UseAuthentication(); // Đảm bảo authentication được dùng trước khi authorization
app.UseAuthorization();

// Cấu hình endpoints cho Areas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Cấu hình routing mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

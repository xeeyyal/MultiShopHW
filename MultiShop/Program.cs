using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>
   opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);
builder.Services.AddScoped<LayoutService>();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllerRoute(
    "default",
    "{area:exists}/{controller=home}/{action=index}/{id?}"
    );
app.MapControllerRoute(
    "default",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();

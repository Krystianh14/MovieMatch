using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MovieMatch.Data;
using MovieMatch.Models;
using MovieMatch.Services;
using System.IO;


var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// DbContext – SQLite / SQL Server (auto)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var cs = connectionString.Trim();
    var isSqlite =
        cs.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase) ||
        cs.Contains(".db", StringComparison.OrdinalIgnoreCase);

    if (isSqlite)
        options.UseSqlite(cs);
    else
        options.UseSqlServer(cs);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<TmdbService>();

var app = builder.Build();

// ✅ Dla SQLite na hostingu: tworzymy bazę automatycznie (bez migracji)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (db.Database.IsSqlite())
    {
        Directory.CreateDirectory("/data");
        db.Database.Migrate();
    }
    else
    {
        db.Database.Migrate();
    }
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();

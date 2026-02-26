using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using WishlistWeb.Data;
using WishlistWeb.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Postgres + EF
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Identity (cookie auth)
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(opt =>
    {
        opt.Password.RequiredLength = 6;
        opt.Password.RequireNonAlphanumeric = false;
        opt.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Account/Login";
    opt.AccessDeniedPath = "/Account/Login";
    opt.LogoutPath = "/Account/Logout";
});

var app = builder.Build();

// ✅ Авто-створення БД + авто-міграції
await EnsureDatabaseCreatedAsync(app.Configuration);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Головна сторінка = Wishlist/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Wishlist}/{action=Index}/{id?}");

app.Run();

static async Task EnsureDatabaseCreatedAsync(IConfiguration cfg)
{
    var cs = cfg.GetConnectionString("Postgres")!;
    var csb = new NpgsqlConnectionStringBuilder(cs);

    var dbName = csb.Database;
    if (string.IsNullOrWhiteSpace(dbName))
        throw new Exception("ConnectionStrings:Postgres має містити Database=...");

    // підключаємось до системної БД
    csb.Database = "postgres";

    await using var conn = new NpgsqlConnection(csb.ConnectionString);
    await conn.OpenAsync();

    await using var check = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @name", conn);
    check.Parameters.AddWithValue("name", dbName);
    var exists = await check.ExecuteScalarAsync() is not null;
    if (exists) return;

    await using var create = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", conn);
    await create.ExecuteNonQueryAsync();
}

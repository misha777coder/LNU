using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WishlistWeb.Models;

namespace WishlistWeb.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<TrashItem> TrashItems => Set<TrashItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<WishlistItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Url).HasMaxLength(1000);
            e.Property(x => x.Note).HasMaxLength(2000);
            e.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
        });

        b.Entity<TrashItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Url).HasMaxLength(1000);
            e.Property(x => x.Note).HasMaxLength(2000);
            e.HasIndex(x => new { x.UserId, x.DeletedAtUtc });
        });
    }
}
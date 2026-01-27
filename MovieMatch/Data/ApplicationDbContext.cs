using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieMatch.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserTitle> UserTitles { get; set; }
    public DbSet<MovieComment> MovieComments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.Nick)
            .IsUnique();

        builder.Entity<UserTitle>()
            .HasOne(ut => ut.User)
            .WithMany(u => u.UserTitles)
            .HasForeignKey(ut => ut.UserId);
        builder.Entity<MovieComment>()
         .HasOne(mc => mc.User)
         .WithMany(u => u.MovieComments)
         .HasForeignKey(mc => mc.UserId);

    }
}

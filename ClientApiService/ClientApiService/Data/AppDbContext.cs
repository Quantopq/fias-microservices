using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClientApiService.Models;
using Microsoft.AspNetCore.Identity;

namespace ClientApiService.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Lead> Leads { get; set; }
    public DbSet<RequestResult> RequestResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Явно указываем схему dbo для всех таблиц Identity
        modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", "dbo");
        modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers", "dbo");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "dbo");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "dbo");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "dbo");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "dbo");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "dbo");

        // Настройка Lead
        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RawAddress).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.ToTable("Leads", "dbo"); // Тоже явно указываем схему
        });

        // Seed ролей (только если таблица пустая)
       // modelBuilder.Entity<IdentityRole>().HasData(
         //   new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
           // new IdentityRole { Id = "2", Name = "Operator", NormalizedName = "OPERATOR" },
            //new IdentityRole { Id = "3", Name = "Viewer", NormalizedName = "VIEWER" }
        //);
    }
}
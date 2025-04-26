using DAL.Configurations;
using DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : IdentityDbContext<UserEntity, RoleEntity, Guid>
{
    public virtual DbSet<UrlEntity> Urls { get; set; } = null!;
    public virtual DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public AppDbContext() { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfigurations());
        modelBuilder.ApplyConfiguration(new UrlEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenEntityTypeConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}

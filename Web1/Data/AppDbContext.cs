using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web1.Data.Entities;
using Web1.Data.Entities.Identity;

namespace Web1.Data;

public class AppDbContext : IdentityDbContext<UserEntity, RoleEntity, int> // параметризація
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

    public DbSet<CategoryEntity> Categories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) // налаштування
    {
        base.OnModelCreating(modelBuilder);

        // identity 
        modelBuilder.Entity<UserRoleEntity>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRoleEntity>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Infrastructure.Identity.Models;

namespace TeamApp.Infrastructure.Identity.Contexts
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");
                entity.Property(m => m.Id).HasMaxLength(50);
                entity.Property(m => m.NormalizedEmail).HasMaxLength(50);
                entity.Property(m => m.NormalizedUserName).HasMaxLength(50);
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
                entity.Property(m => m.Id).HasMaxLength(85);
                entity.Property(m => m.NormalizedName).HasMaxLength(85);
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.Property(m => m.RoleId).HasMaxLength(50);
                entity.Property(m => m.UserId).HasMaxLength(50);
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
                entity.Property(m => m.Id).HasMaxLength(50);
                entity.Property(m => m.UserId).HasMaxLength(50);
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
                entity.Property(m => m.LoginProvider).HasMaxLength(50);
                entity.Property(m => m.ProviderKey).HasMaxLength(50);
                entity.Property(m => m.UserId).HasMaxLength(50);
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(m => m.RoleId).HasMaxLength(50);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
                entity.Property(m => m.UserId).HasMaxLength(50);
                entity.Property(m => m.LoginProvider).HasMaxLength(50);
                entity.Property(m => m.Name).HasMaxLength(50);
            });
        }
    }
}

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

            builder.Entity<ApplicationUser>().Ignore(c => c.AccessFailedCount)
                                           .Ignore(c => c.LockoutEnabled)
                                           
                                           .Ignore(c => c.TwoFactorEnabled)
                                           
                                           
                                           .Ignore(c=>c.AccessFailedCount);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("use_phone_number")
                    .HasColumnType("varchar(20)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("user_created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Dob)
                    .HasColumnName("user_date_of_birth")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Email)
                    .HasColumnName("user_email")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.FullName)
                    .HasColumnName("user_fullname")
                    .HasColumnType("varchar(100)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("user_image_url")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.IsThemeLight).HasColumnName("user_is_theme_light");

                entity.Property(e => e.PasswordHash)
                    .HasColumnName("user_password")
                    .HasColumnType("varchar(500)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");
            }
           );

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

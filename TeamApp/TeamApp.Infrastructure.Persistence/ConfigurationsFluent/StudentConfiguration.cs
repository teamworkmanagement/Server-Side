using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Domain.Entities;

namespace TeamApp.Infrastructure.Persistence.ConfigurationsFluent
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("studentt");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).HasMaxLength(100).IsUnicode();
            builder.Property(s => s.Dob).HasDefaultValue(DateTimeOffset.UtcNow);
        }
    }
}

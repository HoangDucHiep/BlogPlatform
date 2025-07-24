using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Domain.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumo.Infrastructure.Configurations;
internal sealed class SaveChangeVersionConfiguration : IEntityTypeConfiguration<SaveChangeVersion>
{
    public void Configure(EntityTypeBuilder<SaveChangeVersion> builder)
    {
        builder.ToTable("save_change_versions");

        builder.HasKey(version => version.Id);

        builder.Property(v => v.StoryId)
            .IsRequired();

        builder.Property(v => v.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(v => v.Content)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(v => v.Description)
            .HasMaxLength(1000)
            .IsRequired();

        // Navigation properties
        builder.HasOne(v => v.Story)
            .WithMany(s => s.SaveChangeVersions)
            .HasForeignKey(v => v.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(v => v.StoryId);
        builder.HasIndex(v => v.CreatedAtUtc);
        builder.HasIndex(v => new { v.StoryId, v.CreatedAtUtc });
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Domain.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumo.Infrastructure.Configurations;
internal sealed class StoryConfiguration : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.ToTable("stories");

        builder.HasKey(story => story.Id);

        builder.Property(s => s.Title)
            .HasMaxLength(200)
            .IsRequired(true);

        builder.Property(s => s.Content)
            .HasColumnType("text")
            .IsRequired(true);

        builder.Property(s => s.AuthorId)
            .IsRequired();

        builder.Property(s => s.PublicationId)
            .IsRequired(false);

        builder.Property(s => s.Status)
            .HasConversion<int>()
            .HasDefaultValue(StoryStatus.Draft)
            .IsRequired();

        builder.Property(s => s.PublishedAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(s => s.IsPaywalled)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(s => s.ReadTimeCalculated)
            .HasDefaultValue(false)
            .IsRequired();

        // Navigation properties
        builder.HasOne(s => s.Author)
            .WithMany()
            .HasForeignKey(s => s.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.SaveChangeVersions)
            .WithOne(v => v.Story)
            .HasForeignKey(v => v.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => s.Title)
            .HasDatabaseName("IX_Stories_Title");
        builder.HasIndex(s => s.Content);
        builder.HasIndex(s => s.AuthorId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.PublishedAtUtc);
        builder.HasIndex(s => new { s.AuthorId, s.Status });
    }
}

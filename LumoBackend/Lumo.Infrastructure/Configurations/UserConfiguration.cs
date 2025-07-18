using Lumo.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Lumo.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(u => u.UserName)
            .HasMaxLength(100)
            .HasConversion(n => n.Value, value => new Name(value))
            .IsRequired();

        builder.Property(u => u.EmailAddress)
            .HasMaxLength(400)
            .HasConversion(e => e.Value, value => new EmailAddress(value))
            .IsRequired();

        builder.Property(u => u.IdentityId)
            .IsRequired();
        
        builder.Property(u => u.Bio)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(u => u.CoverPictureUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(u => u.SocialLinks)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                value => JsonConvert.DeserializeObject<SocialLinks>(value))
            .IsRequired(false)
            .Metadata.SetValueComparer(new ValueComparer<SocialLinks>(
                (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r),
                v => JsonConvert.SerializeObject(v).GetHashCode(),
                v => JsonConvert.DeserializeObject<SocialLinks>(JsonConvert.SerializeObject(v))
            ));

        builder.HasIndex(u => u.UserName);

        builder.HasIndex(u => u.EmailAddress)
            .IsUnique();

        builder.HasIndex(u => u.IdentityId)
            .IsUnique();

    }
}

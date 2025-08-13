using Lumo.Application.Abstractions.Clock;
using Lumo.Application.Exceptions;
using Lumo.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Lumo.Infrastructure;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    //private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    //{
    //    TypeNameHandling = TypeNameHandling.All
    //};

    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(Entity.CreatedAtUtc))
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();
            }

            if (typeof(IUpdatable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(IUpdatable.LastUpdatedAtUtc))
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();
            }
        }

        base.OnModelCreating(modelBuilder);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            DateTimeOffset now = _dateTimeProvider.UtcNowOffset;

            foreach (EntityEntry<Entity> entry in ChangeTracker.Entries<Entity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(nameof(Entity.CreatedAtUtc)).CurrentValue = now;
                    entry.Property(nameof(IUpdatable.LastUpdatedAtUtc)).CurrentValue = now;
                }
            }

            foreach (EntityEntry<IUpdatable> entry in ChangeTracker.Entries<IUpdatable>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(IUpdatable.LastUpdatedAtUtc)).CurrentValue = now;
                }
            }

            //AddDomainEventsAsOutboxMessages();
            int result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("A concurrency error occurred while saving changes.", ex);
        }
    }

    //private void AddDomainEventsAsOutboxMessages()
    //{
    //    Console.WriteLine("Adding domain events as outbox messages...");
    //}
}

using Lumo.Application.Abstractions.Clock;
using Lumo.Application.Exceptions;
using Lumo.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

        base.OnModelCreating(modelBuilder);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
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

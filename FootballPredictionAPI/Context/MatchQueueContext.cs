using FootballPredictionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballPredictionAPI.Context;

public class MatchQueueContext : DbContext
{
    public MatchQueueContext(DbContextOptions<MatchQueueContext> options) : base(options)
    {
        
    }
    public DbSet<Match> Matches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>(match =>
        {
            match.ToContainer("matchesqueue");
            match.HasPartitionKey(x => x.id);
            match.HasNoDiscriminator();
        });
        base.OnModelCreating(modelBuilder);
    }
}
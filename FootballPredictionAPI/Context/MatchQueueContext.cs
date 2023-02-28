using FootballPredictionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballPredictionAPI.Context;

public class MatchQueueContext : DbContext
{
    public MatchQueueContext(DbContextOptions<MatchQueueContext> options) : base(options)
    {
        
    ***REMOVED***
    public DbSet<Match> Matches { get; set; ***REMOVED***

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>(match =>
        {
            match.ToContainer("matchesqueue");
            match.HasPartitionKey(x => x.id);
            match.HasNoDiscriminator();
        ***REMOVED***);
        base.OnModelCreating(modelBuilder);
    ***REMOVED***
***REMOVED***
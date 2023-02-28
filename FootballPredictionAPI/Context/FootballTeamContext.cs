using FootballPredictionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballPredictionAPI.Context
{
    public class FootballTeamContext : DbContext
    {
        public FootballTeamContext(DbContextOptions<FootballTeamContext> options) : base(options) { ***REMOVED***
        public DbSet<FootballTeam> Teams { get; set; ***REMOVED***
        public DbSet<FootballMatch> Matches { get; set; ***REMOVED***

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FootballTeam>(team =>
            {
                team.ToContainer("teams");
                team.HasPartitionKey(x => x.id);
                team.HasNoDiscriminator();
            ***REMOVED***);
            modelBuilder.Entity<FootballMatch>(match =>
            {
                match.ToContainer("matches");
                match.HasPartitionKey(x => x.id);
                match.HasNoDiscriminator();
            ***REMOVED***);
            base.OnModelCreating(modelBuilder);
        ***REMOVED***
    ***REMOVED***
***REMOVED***

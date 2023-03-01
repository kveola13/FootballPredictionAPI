using FootballPredictionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballPredictionAPI.Context
{
    public class FootballTeamContext : DbContext
    {
        public FootballTeamContext(DbContextOptions<FootballTeamContext> options) : base(options) { }
        public DbSet<FootballTeam> Teams { get; set; }
        public DbSet<FootballMatch> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FootballTeam>(team =>
            {
                team.ToContainer("teams");
                team.HasPartitionKey(x => x.id);
                team.HasNoDiscriminator();
            });
            modelBuilder.Entity<FootballMatch>(match =>
            {
                match.ToContainer("matches");
                match.HasPartitionKey(x => x.id);
                match.HasNoDiscriminator();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}

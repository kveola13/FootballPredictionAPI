using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace FootballPredictionAPI.Context
{
    public class FootballTeamContext : DbContext
    {
        IConfiguration _configuration;
        public FootballTeamContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public FootballTeamContext(DbContextOptions<FootballTeamContext> options) : base(options) { }
        public DbSet<FootballTeam> Teams { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseCosmos(
                accountEndpoint: _configuration.GetConnectionString("COSMOS_ENDPOINT"),
                accountKey: _configuration.GetConnectionString("COSMOS_KEY"),
                "FootballTeams");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<FootballTeam>().ToContainer("Teams").HasPartitionKey(nameof(FootballTeam.Id));
        }
        
    }
}

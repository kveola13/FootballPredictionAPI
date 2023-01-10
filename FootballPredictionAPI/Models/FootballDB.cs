using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballPredictionAPI.Models
{
    public class FootballDB : DbContext
    {
        public FootballDB(DbContextOptions<FootballDB> options) : base(options) { ***REMOVED***
        public DbSet<FootballTeam> FootballDBs { get; set; ***REMOVED***
        public DbSet<FootballTeamDTO> FootballInits {  get; set; ***REMOVED***

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FootballTeam>().HasData(
                new FootballTeam
                {
                    Id = 1,
                    Name = "FC Barcelona",
                    Points = 10,
                    MatchesWon = 3,
                    MatchesLost = 2,
                    MatchesDraw = 1,
                    Description = "Team from Barcelona"
                ***REMOVED***
            );
           
        ***REMOVED***
    ***REMOVED***
***REMOVED***

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballPredictionAPI.Models
{
    public class FootballTeamContext : DbContext
    {
        public FootballTeamContext(DbContextOptions<FootballTeamContext> options) : base(options) { ***REMOVED***
        public DbSet<FootballTeam> Teams { get; set; ***REMOVED***
    ***REMOVED***
***REMOVED***

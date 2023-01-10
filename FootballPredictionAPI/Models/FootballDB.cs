using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballPredictionAPI.Models
{
    public class FootballDB : DbContext
    {
        public FootballDB(DbContextOptions<FootballDB> options) : base(options) { }
        public DbSet<FootballTeam> FootballDBs { get; set; }
        public DbSet<FootballTeamDTO> FootballInits {  get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballPredictionAPI.Models
{
    public class FootballDB : DbContext
    {
        public FootballDB(DbContextOptions<FootballDB> options) : base(options) { ***REMOVED***
        public DbSet<FootballTeam> FootballDBs { get; set; ***REMOVED***
        public DbSet<FootballTeamDTO> FootballInits {  get; set; ***REMOVED***
    ***REMOVED***
***REMOVED***

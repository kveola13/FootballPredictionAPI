using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Models;

namespace FootballPredictionAPI.Interfaces;

public interface IFootballRepository
{
    Task<IEnumerable<FootballTeamDTO?>> GetFootballTeams();
    Task<FootballTeamDTO?> GetFootballTeamById(string id);
    Task<FootballTeamDTO?> GetFootballTeamByName(string name);
    Task<FootballTeam?> UpdateFootballTeam(string id, FootballTeam footballTeam);
    Task<FootballTeam?> AddFootballTeam(FootballTeamDTO footballTeamDTO);
    Task<FootballTeam?> DeleteFootballTeamById(string id);
    Task<FootballTeam?> DeleteFootballTeamByName(string name);
    [Obsolete("This will no longer be needed after a CosmosDB integration")]
    Task<IEnumerable<FootballTeamDTO>> Seed();
    int CalculatePoints(FootballTeam footballTeam);
    [Obsolete("Not needed after update")]
    bool FootballTeamTableExists();
***REMOVED***
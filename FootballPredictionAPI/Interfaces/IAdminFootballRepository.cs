using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballPredictionAPI.Interfaces;
public interface IAdminFootballRepository
{
    Task<IEnumerable<FootballTeam?>> GetFootballTeams();
    Task<FootballTeam?> GetFootballTeamById(string id);
    Task<FootballTeam?> GetFootballTeamByName(string name);
    Task<FootballTeam?> UpdateFootballTeam(string id, FootballTeam footballTeam);
    Task<FootballTeam?> AddFootballTeam(FootballTeam footballTeamDTO);
    Task<FootballTeam?> DeleteFootballTeamById(string id);
    Task<FootballTeam?> DeleteFootballTeamByName(string name);
    [Obsolete("This will no longer be needed after a CosmosDB integration")]
    Task<IEnumerable<FootballTeam>> Seed();
    int CalculatePoints(FootballTeam footballTeam);
    Task<ActionResult<string>> PredictResult(string team1, string team2);
***REMOVED***

using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballPredictionAPI.Interfaces;

public interface IFootballCosmosRepository
{
    public IEnumerable<FootballTeamDTO> GetFootballTeamDtos();
    Task<FootballTeamDTO?> GetFootballTeamById(string id);
    Task<FootballTeamDTO?> GetFootballTeamByName(string name);
    FootballTeam? UpdateFootballTeam(string id, FootballTeam footballTeam);
    bool AddFootballTeam(FootballTeamDTO footballTeamDTO);
    FootballTeam? DeleteFootballTeamById(string id);
    FootballTeam? DeleteFootballTeamByName(string name);
    public Task<List<FootballTeam>> GetFootballTeams();

    int CalculatePoints(FootballTeam footballTeam);

    Task PopulateMatches();
    Task<ActionResult<string>> PredictResult(string team1, string team2);
    Task<IEnumerable<Match>> GetNewMatches();
    void PopulateMatchesToCome();
    FootballMatch ReadStatsForMatch(Match match);
    bool AddFootballMatchWithStats(FootballMatch footballMatchesWithStats);
    IEnumerable<Match> DeleteFromQueue(IEnumerable<Match> matchesToDelete);
    FootballTeam? UpdateHomeTeam(FootballMatch footballMatch, FootballTeam footballTeam);
    FootballTeam? UpdateAwayTeam(FootballMatch footballMatch, FootballTeam footballTeam);
    Task<FootballTeam?> GetTeamByName(string teamName);
    bool AddTeam(FootballTeam ft);
    IEnumerable<Match> GetMatchesQueue();
***REMOVED***
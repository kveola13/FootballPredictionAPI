using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Mvc;

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

    int CalculatePoints(FootballTeam footballTeam);
    [Obsolete("Not needed after update")]
    bool FootballTeamTableExists();

    void PopulateTeams();
    Task PopulateMatches();
    Task<ActionResult<string>> PredictResult(string team1, string team2);
    Task<IEnumerable<Match>> GetNewMatches();
    Task PopulateMatchesToCome();
    Task<FootballMatch> ReadStatsForMatch(Match match);
    Task<FootballMatch?> AddFootballMatchWithStats(FootballMatch footballMatchesWithStats);
    Task<IEnumerable<Match>> DeleteFromQueue(IEnumerable<Match> matchesToDelete);
    FootballTeam? UpdateHomeTeam(FootballMatch footballMatch, FootballTeam footballTeam);
    Task<FootballTeam?> UpdateAwayTeam(FootballMatch footballMatch, FootballTeam footballTeam);
    Task<FootballTeam?> GetTeamByName(string teamName);
    Task<FootballTeam?> AddTeam(FootballTeam ft);
}
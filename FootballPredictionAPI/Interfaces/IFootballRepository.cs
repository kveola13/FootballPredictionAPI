using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballPredictionAPI.Interfaces;

public interface IFootballRepository
{
    Task<IEnumerable<FootballTeamDTO>> GetFootballTeams();
    Task<FootballTeamDTO> GetFootballTeamById(int it);
    Task<FootballTeamDTO> GetFootballTeamByName(string name);
    Task<bool> UpdateFootballTeam(int id, FootballTeam footballTeamDto);
    Task<IEnumerable<FootballTeamDTO>> UpdateMatchesWIthResult(string team1, string team2, string result);
    Task<bool> DeleteFootballTeamById(int id);
    Task<bool> DeleteFootballTeamByName(string name);
    Task<bool> AddFootballTeam(FootballTeam footballTeam);
    Task<FootballTeamDTO> Seed();
    Task<bool> Exists<T>(T id);
    int CalculatePoints(FootballTeam footballTeam);
    bool ListEmpty();
    bool FootballTeamTableExists();
}
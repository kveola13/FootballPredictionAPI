using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace FootballPredictionAPI.Interfaces;

public interface IFootballRepository
{
    Task<IEnumerable<FootballTeamDTO>> GetFootballTeams();
    Task<FootballTeamDTO> GetFootballTeamById(string id);
    Task<FootballTeamDTO> GetFootballTeamByName(string name);
    Task<bool> UpdateFootballTeam(string id, FootballTeam footballTeamDto);
    Task<bool> DeleteFootballTeamById(string id);
    Task<bool> DeleteFootballTeamByName(string name);
    Task<bool> AddFootballTeam(FootballTeam footballTeam);
    Task<IEnumerable<FootballTeamDTO>> Seed();
    int CalculatePoints(FootballTeam footballTeam);
    bool ListEmpty();
    bool FootballTeamTableExists();
}
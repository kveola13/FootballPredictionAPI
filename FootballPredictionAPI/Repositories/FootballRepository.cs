using AutoMapper;
using FootballPredictionAPI.Context;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FootballPredictionAPI.Repositories;

public class FootballRepository : IFootballRepository
{

    private readonly FootballTeamContext _context;
    private readonly IMapper _mapper;
    
    public FootballRepository(FootballTeamContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<FootballTeamDTO>> GetFootballTeams()
    {
        var list = await _context.Teams.ToListAsync();
        return _mapper.Map<IEnumerable<FootballTeamDTO>>(list);
    }

    public async Task<FootballTeamDTO> GetFootballTeamById(int id)
    {
        Task<bool> exists = Exists<int>(id);
        Console.WriteLine(exists.Result);
        var team = await _context.Teams.FirstOrDefaultAsync(ft => ft.Id == id);
        return _mapper.Map<FootballTeamDTO>(team);
    }

    public async Task<FootballTeamDTO> GetFootballTeamByName(string name)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(ft => ft.Name.ToLower().Equals(name.ToLower()));
        return _mapper.Map<FootballTeamDTO>(team);
    }

    public async Task<bool> UpdateFootballTeam(int id, FootballTeam footballTeam)
    {
        footballTeam.Points = CalculatePoints(footballTeam);
        _context.Teams.Update(footballTeam);
        int result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteFootballTeamById(int id)
    {
        _context.Teams.Remove(_context.Teams.FirstOrDefault(ft => ft.Id == id));
        int result = await _context.SaveChangesAsync();
        return result > 0; 
    }

    public async Task<bool> DeleteFootballTeamByName(string name)
    {
        _context.Teams.Remove(_context.Teams.FirstOrDefault(ft => ft.Name.ToLower().Equals(name.ToLower())));
        int result = await _context.SaveChangesAsync();
        return result > 0; 
    }

    public async Task<bool> AddFootballTeam(FootballTeam footballTeam)
    {
        await _context.Teams.AddAsync(footballTeam);
        int result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<FootballTeamDTO>> Seed()
    {
        string[] footballTeamsNames = new[] { "Real Madrid", "Real Sociedad", "Real Betis", 
            "Atletico Madrid", "Villareal", "Athletico Bilbao", "Valencia FC", "Fc Barcelona", "Sevilla FC" };

        List<FootballTeam> teams = new List<FootballTeam>();

        Random r = new Random();
        foreach (string ftName in footballTeamsNames)
        {
            int wins = r.Next(7);
            Console.WriteLine(wins);
            int lost = r.Next(7-wins);
            Console.WriteLine(lost);
            int draw = r.Next(7-wins-lost);
            Console.WriteLine(draw);
            FootballTeam team = new FootballTeam
            {
                Name = ftName,
                MatchesWon = wins,
                MatchesLost = lost,
                MatchesDraw = draw,
                Description = $"Team located in Spain: {ftName}"
            };
            team.Points = CalculatePoints(team);
            teams.Add(team);
            await _context.Teams.AddAsync(team);
        }
        
        await _context.SaveChangesAsync();
        
        //var teams = await _context.Teams.ToListAsync();
        return _mapper.Map<IEnumerable<FootballTeamDTO>>(teams);
        
    }

    public async Task<bool> Exists<T>(T id)
    {
        return await _context.Teams.AnyAsync(ft => ft.Id.ToString() == id.ToString() || ft.Name.ToLower().Equals(id.ToString().ToLower()));
    }

    public int CalculatePoints(FootballTeam footballTeam)
    {
        return (footballTeam.MatchesWon * 3) + footballTeam.MatchesDraw;
    }

    public bool ListEmpty()
    {
        return !_context.Teams.Any();
    }

    public bool FootballTeamTableExists()
    {
        return _context.Teams != null;
    }
}
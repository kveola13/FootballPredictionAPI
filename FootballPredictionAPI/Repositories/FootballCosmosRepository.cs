using AutoMapper;
using FootballPredictionAPI.Context;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballPredictionAPI.Repositories;

public class FootballCosmosRepository : IFootballCosmosRepository
{
    private readonly FootballTeamContext _context;
    private readonly MatchQueueContext _matchesContext;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly string containerName = "teams";
    private readonly string macthesContainer = "matches";
    private readonly string queueContainer = "matchesqueue";
    private readonly WebCrawler.WebCrawler _webCrawler;

    public FootballCosmosRepository(FootballTeamContext _context,MatchQueueContext _matchesContext, IMapper _mapper, IConfiguration configuration)
    {
        this._context = _context;
        this._matchesContext = _matchesContext;
        this._mapper = _mapper;
        _configuration = configuration;
        _webCrawler = new WebCrawler.WebCrawler();
    ***REMOVED***


    public IEnumerable<FootballTeamDTO> GetFootballTeamDtos()
    {
        return _mapper.Map<IEnumerable<FootballTeamDTO>>(_context.Teams.ToList());
    ***REMOVED***

    public async Task<FootballTeamDTO?> GetFootballTeamById(string id)
    {
        FootballTeam team = await _context.Teams.FirstOrDefaultAsync(t => t.id == id);
        return team != null ? _mapper.Map<FootballTeamDTO>(team) : null;
    ***REMOVED***

    public async Task<FootballTeamDTO?> GetFootballTeamByName(string name)
    {
        FootballTeam team = await _context.Teams.FirstOrDefaultAsync(t => t.Name.ToLower().Equals(name.ToLower()));
        return team != null ? _mapper.Map<FootballTeamDTO>(team) : null;
    ***REMOVED***

    public FootballTeam? UpdateFootballTeam(string id, FootballTeam footballTeam)
    {
        
        FootballTeam team = new FootballTeam
        {
            id = footballTeam.id,
            Name = footballTeam.Name,
            MatchesWon = footballTeam.MatchesWon,
            MatchesLost = footballTeam.MatchesLost,
            MatchesDraw = footballTeam.MatchesDraw,
            Description = footballTeam.Description,
            GoalsScored = footballTeam.GoalsScored,
            GoalsLost = footballTeam.GoalsLost,
            MatchesPlayed = footballTeam.MatchesPlayed
        ***REMOVED***;
        team.Points = CalculatePoints(team);
        team.GoalDifference = team.GoalsScored - team.GoalsLost;

        var teamToUpdate = _context.Teams.FirstOrDefault(t => t.id == id);
        if (teamToUpdate == null)
        {
            _context.Teams.Add(team);
        ***REMOVED***
        else
        {
            _context.Teams.Update(team);
        ***REMOVED***

        _context.SaveChanges();
        return team;
    ***REMOVED***

    public bool AddFootballTeam(FootballTeamDTO footballTeamDTO)
    {
        bool exists = _context.Teams.Any(t => t.Name.ToLower().Equals(footballTeamDTO.Name));
        if (exists)
        {
            return false;
        ***REMOVED***

        var newTeam = _mapper.Map<FootballTeam>(footballTeamDTO);
        newTeam.id = Guid.NewGuid().ToString();
        newTeam.Points = CalculatePoints(newTeam);
        _context.Add(newTeam);
        int result = _context.SaveChanges();

        return result > 0;
    ***REMOVED***

    public FootballTeam? DeleteFootballTeamById(string id)
    {
        var teamToDelete = _context.Teams.FirstOrDefault(t => t.id == id);
        if (teamToDelete == null)
        {
            return null;
        ***REMOVED***

        _context.Teams.Remove(teamToDelete);
        _context.SaveChanges();
        return teamToDelete;
    ***REMOVED***

    public FootballTeam? DeleteFootballTeamByName(string name)
    {
        var teamToDelete = _context.Teams.FirstOrDefault(t => t.Name.ToLower().Equals(name.ToLower()));
        if (teamToDelete == null)
        {
            return null;
        ***REMOVED***

        _context.Teams.Remove(teamToDelete);
        _context.SaveChanges();
        return teamToDelete;
    ***REMOVED***

    public async Task<List<FootballTeam>> GetFootballTeams()
    {
        return await _context.Teams.ToListAsync();
    ***REMOVED***

    public int CalculatePoints(FootballTeam footballTeam)
    {
        return (footballTeam.MatchesWon * 3) + footballTeam.MatchesDraw;
    ***REMOVED***
    
    public Task PopulateMatches()
    {
        throw new NotImplementedException();
    ***REMOVED***

    public Task<ActionResult<string>> PredictResult(string team1, string team2)
    {
        throw new NotImplementedException();
    ***REMOVED***

    public Task<IEnumerable<Match>> GetNewMatches()
    {
        throw new NotImplementedException();
    ***REMOVED***

    public void PopulateMatchesToCome()
    {
        var matchDays = _webCrawler.GetMatchDays();
        List<Match> matches = new();
        foreach (var matchday in matchDays)
        {
            var lines = _webCrawler.GetMatchDayResults(matchday);
            var linesToMatches = new List<Match>();
            foreach (var data in lines)
            {
                if (!data[3].Any(char.IsDigit))
                {
                    var m = new Match();
                    m.ReadValues(data);
                    linesToMatches.Add(m);
                ***REMOVED***
            ***REMOVED***
            matches = matches.Concat(linesToMatches).ToList();
        ***REMOVED***

        foreach (Match match in matches)
        {
            match.id = Guid.NewGuid().ToString();
            _matchesContext.Matches.Add(match);
        ***REMOVED***
    ***REMOVED***

    public FootballMatch ReadStatsForMatch(Match match)
    {
        FootballMatch fm = _webCrawler.ReadStatsForMatch(match);
        return fm;
    ***REMOVED***

    public bool AddFootballMatchWithStats(FootballMatch footballMatchesWithStats)
    {
        footballMatchesWithStats.id = Guid.NewGuid().ToString();
        _context.Matches.Add(footballMatchesWithStats);
        int result = _context.SaveChanges();
        return result > 0;
    ***REMOVED***

    public IEnumerable<Match> DeleteFromQueue(IEnumerable<Match> matchesToDelete)
    {
        List<Match> deleted = new();
        foreach (Match match in matchesToDelete)
        {
            var matchObject = _context.Matches.FirstOrDefault(m => m.id.Equals(match.id));
            if (matchObject != null)
            {
                _context.Matches.Remove(matchObject);
                int result = _context.SaveChanges();
                if (result > 0)
                {
                    deleted.Add(matchObject);
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***

        return deleted;
    ***REMOVED***

    public FootballTeam? UpdateHomeTeam(FootballMatch m, FootballTeam t)
    {
        t.MatchesWon += m.HTGoals > m.ATGoals ? 1 : 0;
        t.MatchesLost += m.HTGoals < m.ATGoals ? 1 : 0;
        t.MatchesDraw += m.HTGoals == m.ATGoals ? 1 : 0;
        t.GoalsScored += (int)m.HTGoals;
        t.GoalsLost += (int)m.ATGoals;
        t.MatchesPlayed += 1;
        t.Points = CalculatePoints(t);
        t.GoalDifference = t.GoalsScored - t.GoalsLost;
        return t;
    ***REMOVED***

    public FootballTeam? UpdateAwayTeam(FootballMatch footballMatch, FootballTeam footballTeam)
    {
        throw new NotImplementedException();
    ***REMOVED***

    public async Task<FootballTeam?> GetTeamByName(string teamName)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(t => t.Name.ToLower().Equals(teamName.ToLower()));
        return team;
    ***REMOVED***

    public bool AddTeam(FootballTeam ft)
    {
        ft.id = Guid.NewGuid().ToString();
        _context.Teams.AddAsync(ft);
        int result = _context.SaveChanges();
        return result > 0;
    ***REMOVED***

    public IEnumerable<Match> GetMatchesQueue()
    {
        return _matchesContext.Matches.ToList();
    ***REMOVED***
***REMOVED***
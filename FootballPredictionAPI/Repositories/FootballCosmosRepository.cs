using System.Net.Http.Headers;
using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FootballPredictionAPI.Context;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using NuGet.Protocol;

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
    }


    public IEnumerable<FootballTeamDTO> GetFootballTeamDtos()
    {
        return _mapper.Map<IEnumerable<FootballTeamDTO>>(_context.Teams.ToList());
    }

    public FootballTeamDTO? GetFootballTeamById(string id)
    {
        FootballTeam team = _context.Teams.FirstOrDefault(t => t.id == id);
        return team != null ? _mapper.Map<FootballTeamDTO>(team) : null;
    }

    public FootballTeamDTO? GetFootballTeamByName(string name)
    {
        FootballTeam team = _context.Teams.FirstOrDefault(t => t.Name.ToLower().Equals(name.ToLower()));
        return team != null ? _mapper.Map<FootballTeamDTO>(team) : null;
    }

    public FootballTeam? UpdateFootballTeam(string id, FootballTeam footballTeam)
    {

        FootballTeam teamToUpdate = _context.Teams.FirstOrDefault(t => t.id == id);
        if (teamToUpdate == null)
        {
            teamToUpdate = new FootballTeam
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
            };
            
            teamToUpdate.Points = CalculatePoints(teamToUpdate);
            teamToUpdate.GoalDifference = teamToUpdate.GoalsScored - teamToUpdate.GoalsLost;
            _context.Teams.Add(teamToUpdate);
        }
        else
        {
            teamToUpdate.Name = footballTeam.Name;
            teamToUpdate.MatchesWon = footballTeam.MatchesWon;
            teamToUpdate.MatchesLost = footballTeam.MatchesLost;
            teamToUpdate.MatchesDraw = footballTeam.MatchesDraw;
            teamToUpdate.Description = footballTeam.Description;
            teamToUpdate.GoalsScored = footballTeam.GoalsScored;
            teamToUpdate.GoalsLost = footballTeam.GoalsLost;
            teamToUpdate.MatchesPlayed = footballTeam.MatchesPlayed;
            teamToUpdate.Points = CalculatePoints(teamToUpdate);
            teamToUpdate.GoalDifference = teamToUpdate.GoalsScored - teamToUpdate.GoalsLost;
            _context.Teams.Update(teamToUpdate);
        }

        _context.SaveChanges();
        return teamToUpdate;
    }

    public bool AddFootballTeam(FootballTeamDTO footballTeamDTO)
    {
        bool exists = _context.Teams.Any(t => t.Name.ToLower().Equals(footballTeamDTO.Name));
        if (exists)
        {
            return false;
        }

        var newTeam = _mapper.Map<FootballTeam>(footballTeamDTO);
        newTeam.id = Guid.NewGuid().ToString();
        newTeam.Points = CalculatePoints(newTeam);
        _context.Add(newTeam);
        int result = _context.SaveChanges();

        return result > 0;
    }

    public FootballTeam? DeleteFootballTeamById(string id)
    {
        var teamToDelete = _context.Teams.FirstOrDefault(t => t.id == id);
        if (teamToDelete == null)
        {
            return null;
        }

        _context.Teams.Remove(teamToDelete);
        _context.SaveChanges();
        return teamToDelete;
    }

    public FootballTeam? DeleteFootballTeamByName(string name)
    {
        var teamToDelete = _context.Teams.FirstOrDefault(t => t.Name.ToLower().Equals(name.ToLower()));
        if (teamToDelete == null)
        {
            return null;
        }

        _context.Teams.Remove(teamToDelete);
        _context.SaveChanges();
        return teamToDelete;
    }

    public async Task<List<FootballTeam>> GetFootballTeams()
    {
        return await _context.Teams.ToListAsync();
    }

    public int CalculatePoints(FootballTeam footballTeam)
    {
        return (footballTeam.MatchesWon * 3) + footballTeam.MatchesDraw;
    }
    
    public Task PopulateMatches()
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult<string>> PredictResult(string team1, string team2)
    {
        List<MatchTeam> requestData = new();

        var matchesTeam1 = _context.Matches.Where(m => m.HomeTeam.Equals(team1) || m.AwayTeam.Equals(team1)).OrderByDescending(m => m.Date).Take(5);
        var matchesTeam2 = _context.Matches.Where(m => m.HomeTeam.Equals(team1) || m.AwayTeam.Equals(team1)).OrderByDescending(m => m.Date).Take(5);
        
        foreach (var match in matchesTeam1)
        {
            var nm = new MatchTeam
            {
                Date = match.Date,
                Team = team1,
                Opponent = match.HomeTeam == team1 ? match.AwayTeam : match.HomeTeam,
                Possession = match.HomeTeam == team1 ? match.HTPossession : match.ATPossession,
                Totalshots = match.HomeTeam == team1 ? match.HTTotalshots : match.ATTotalshots,
                Accuaracy = match.HomeTeam == team1 ? match.HTAccuaracy : match.ATAccuaracy,
                Fouls = match.HomeTeam == team1 ? match.HTFouls : match.ATFouls,
                Yellowcards = match.HomeTeam == team1 ? match.HTYellowcards : match.ATYellowcards,
                Redcards = match.HomeTeam == team1 ? match.HTRedcards : match.ATRedcards,
                Offsides = match.HomeTeam == team1 ? match.HTOffsides : match.ATOffsides,
                Cornerstaken = match.HomeTeam == team1 ? match.HTCornerstaken : match.ATCornerstaken,
                Goals = match.HomeTeam == team1 ? match.HTGoals : match.ATGoals,
                OpponentGoals = match.HomeTeam == team1 ? match.ATGoals : match.HTGoals
            };
            requestData.Add(nm);
        }
        foreach (var match in matchesTeam2)
        {
            var nm = new MatchTeam
            {
                Date = match.Date,
                Team = team2,
                Opponent = match.HomeTeam == team2 ? match.AwayTeam : match.HomeTeam,
                Possession = match.HomeTeam == team2 ? match.HTPossession : match.ATPossession,
                Totalshots = match.HomeTeam == team2 ? match.HTTotalshots : match.ATTotalshots,
                Accuaracy = match.HomeTeam == team2 ? match.HTAccuaracy : match.ATAccuaracy,
                Fouls = match.HomeTeam == team2 ? match.HTFouls : match.ATFouls,
                Yellowcards = match.HomeTeam == team2 ? match.HTYellowcards : match.ATYellowcards,
                Redcards = match.HomeTeam == team2 ? match.HTRedcards : match.ATRedcards,
                Offsides = match.HomeTeam == team2 ? match.HTOffsides : match.ATOffsides,
                Cornerstaken = match.HomeTeam == team2 ? match.HTCornerstaken : match.ATCornerstaken,
                Goals = match.HomeTeam == team2 ? match.HTGoals : match.ATGoals,
                OpponentGoals = match.HomeTeam == team2 ? match.ATGoals : match.HTGoals
            };
            requestData.Add(nm);
        }

        foreach (var m in requestData)
        {
            Console.WriteLine(m);
        }

        var request = requestData.ToJson();
        Console.WriteLine(request);
        
        var handler = new HttpClientHandler()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
        };
        using var client = new HttpClient(handler);
        // Request data goes here
        // The example below assumes JSON formatting which may be updated
        // depending on the format your endpoint expects.
        // More information can be found here:
        // https://docs.microsoft.com/azure/machine-learning/how-to-deploy-advanced-entry-script
        var requestBody = @"{
                  ""Inputs"": {
                    ""input1"": @input
                  },
                  ""GlobalParameters"": {}
                }".Replace("@input", request);

        // Replace this with the primary/secondary key or AMLToken for the endpoint

        if (string.IsNullOrEmpty(StringConstrains.PredictionAPIKey))
        {
            throw new Exception("A key should be provided to invoke the endpoint");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", StringConstrains.PredictionAPIKey);
        client.BaseAddress = new Uri(StringConstrains.PredictionUrl);

        var content = new StringContent(requestBody);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        // WARNING: The 'await' statement below can result in a deadlock
        // if you are calling this code from the UI thread of an ASP.Net application.
        // One way to address this would be to call ConfigureAwait(false)
        // so that the execution does not attempt to resume on the original context.
        // For instance, replace code such as:
        //      result = await DoSomeTask()
        // with the following:
        //      result = await DoSomeTask().ConfigureAwait(false)
        HttpResponseMessage responsePrediction = await client.PostAsync("", content);

        if (responsePrediction.IsSuccessStatusCode)
        {
            string result = await responsePrediction.Content.ReadAsStringAsync();
            string predictions = String.Format("Result: {0}", result);
            return predictions;
        }
        else
        {
            string responseContent = await responsePrediction.Content.ReadAsStringAsync();
            return string.Format("The request failed with status code: {0}", responsePrediction.StatusCode);
        }
    }

    public Task<IEnumerable<Match>> GetNewMatches()
    {
        throw new NotImplementedException();
    }

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
                }
            }
            matches = matches.Concat(linesToMatches).ToList();
        }

        foreach (Match match in matches)
        {
            match.id = Guid.NewGuid().ToString();
            _matchesContext.Matches.Add(match);
        }
    }

    public FootballMatch ReadStatsForMatch(Match match)
    {
        FootballMatch fm = _webCrawler.ReadStatsForMatch(match);
        return fm;
    }

    public bool AddFootballMatchWithStats(FootballMatch footballMatchesWithStats)
    {
        footballMatchesWithStats.id = Guid.NewGuid().ToString();
        _context.Matches.Add(footballMatchesWithStats);
        int result = _context.SaveChanges();
        return result > 0;
    }

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
                }
            }
        }

        return deleted;
    }

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
    }

    public FootballTeam? UpdateAwayTeam(FootballMatch footballMatch, FootballTeam footballTeam)
    {
        throw new NotImplementedException();
    }

    public async Task<FootballTeam?> GetTeamByName(string teamName)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(t => t.Name.ToLower().Equals(teamName.ToLower()));
        return team;
    }

    public bool AddTeam(FootballTeam ft)
    {
        ft.id = Guid.NewGuid().ToString();
        _context.Teams.AddAsync(ft);
        int result = _context.SaveChanges();
        return result > 0;
    }

    public IEnumerable<Match> GetMatchesQueue()
    {
        return _matchesContext.Matches.ToList();
    }
}
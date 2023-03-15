using System.Globalization;
using System.Net.Http.Headers;
using AutoMapper;
using CsvHelper;
using FootballPredictionAPI.Context;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
    
    [Obsolete("Not needed after initial population of db")]
    public void PopulateMatches()
    {
        // Add matches
        IEnumerable<FootballMatch> records;
        using (var reader = new StreamReader("Data/matches_teams_current.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            records = csv.GetRecords<FootballMatch>().ToList();
        }

        records = records.Skip(1);

        foreach (var record in records)
        {
            
            // Here add to db
            record.id = Guid.NewGuid().ToString();
            _context.Matches.Add(record);
        }
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

        string normalisedData = null;
        using (var client = new HttpClient(handler))
        {
            
            var requestBody = @"{
                  ""Inputs"": {
                    ""input1"": @request
                  },
                  ""GlobalParameters"": {}
                }".Replace("@request", request);

            // Replace this with the primary/secondary key or AMLToken for the endpoint
            string apiKey = StringConstrains.NormalizationAPIKey;
            if (string.IsNullOrEmpty(apiKey))  
            {
                throw new Exception("A key should be provided to invoke the endpoint");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", apiKey);
            client.BaseAddress = new Uri(StringConstrains.NormalizationUrl);

            var content = new StringContent(requestBody);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            HttpResponseMessage response = await client.PostAsync("", content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Result: {0}", result);
                normalisedData = result;
            }
            else
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                
                return string.Format("The request failed with status code: {0}", response.StatusCode);
            }
        }

        if (normalisedData == null)
        {
            return "Wasn't able to deserialize data";
        }

        return null;
    }

    public IEnumerable<Match> GetNewMatches()
    {
        var URIs = _matchesContext.Matches.AsEnumerable().Where(m => m.Date < DateTime.Now.Date).OrderBy(m => m.Date).Take(1);
        return URIs;
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
            var matchObject = _matchesContext.Matches.FirstOrDefault(m => m.id.Equals(match.id));
            if (matchObject != null)
            {
                _matchesContext.Matches.Remove(matchObject);
                int result = _matchesContext.SaveChanges();
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

    public FootballTeam? UpdateAwayTeam(FootballMatch m, FootballTeam t)
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

    public FootballTeam? GetTeamByName(string teamName)
    {
        var team = _context.Teams.FirstOrDefault(t => t.Name.ToLower().Equals(teamName.ToLower()));
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
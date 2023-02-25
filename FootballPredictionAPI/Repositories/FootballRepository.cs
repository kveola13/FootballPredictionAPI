using System.Globalization;
using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FootballPredictionAPI.Context;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Net.Http.Headers;
using CsvHelper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using NuGet.Protocol;
using File = System.IO.File;

namespace FootballPredictionAPI.Repositories;

public class FootballRepository : IFootballRepository
{
    private readonly FootballTeamContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly string containerName = "teams";
    private readonly string macthesContainer = "matches";
    private readonly string queueContainer = "matchesqueue";
    private readonly WebCrawler.WebCrawler _webCrawler;

    public FootballRepository(FootballTeamContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
        _webCrawler = new WebCrawler.WebCrawler();
    }

    public async Task<IEnumerable<Match>> GetNewMatches()
    {
        CreateQueueConnection(out _, out Container container);

        string today = DateTime.Now.Date.ToString();
        
        QueryDefinition query = new($"select top 1 * from c where c.Date < {today} order by c.Date");
        var dbContainerResponse = container.GetItemQueryIterator<Match>(query);
        List<Match> URIs = new();
        while (dbContainerResponse.HasMoreResults)
        {
            FeedResponse<Match> response = await dbContainerResponse.ReadNextAsync();
            foreach (var match in response)
            {
                URIs.Add(match);
            }
        }
        return URIs;
    }

    public FootballMatch ReadStatsForMatch(Match match)
    {
        FootballMatch fm = _webCrawler.ReadStatsForMatch(match);
        return fm;
    }

    public async Task<FootballMatch?> AddFootballMatchWithStats(FootballMatch footballMatchesWithStats)
    {
        CreateContainerMatches(out _, out Container container);
        footballMatchesWithStats.Id = Guid.NewGuid().ToString();
        var createResponse = await container.CreateItemAsync(footballMatchesWithStats);
        return createResponse;
    }

    public async Task<IEnumerable<Match>> DeleteFromQueue(IEnumerable<Match> matchesToDelete)
    {
        CreateQueueConnection(out _, out Container container);
        List<Match> deleted = new();
        foreach (var m in matchesToDelete)
        {
            IOrderedQueryable<Match> queryable = container.GetItemLinqQueryable<Match>();
            var matches = queryable
                .Where(fm => fm.Id!.Equals(m.Id));
            using FeedIterator<Match> linqFeed = matches.ToFeedIterator();
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<Match> response = await linqFeed.ReadNextAsync();
                
                var match = response.FirstOrDefault();
                var resp = await container.DeleteItemAsync<Match>(match!.Id, new PartitionKey(match!.Id));
                if (resp != null)
                {
                    deleted.Add(match);
                }
            }
        }
        return deleted;
    }

    public FootballTeam? UpdateAwayTeam(FootballMatch m, FootballTeam t)
    {
        t.MatchesWon += m.ATGoals > m.HTGoals ? 1 : 0;
        t.MatchesLost += m.ATGoals < m.HTGoals ? 1 : 0;
        t.MatchesDraw += m.ATGoals == m.HTGoals ? 1 : 0;
        t.GoalsScored += (int)m.ATGoals;
        t.GoalsLost += (int)m.HTGoals;
        t.MatchesPlayed += 1;
        t.Points = CalculatePoints(t);
        t.GoalDifference = t.GoalsScored - t.GoalsLost;
        return t;
    }

    [Obsolete("One time job & has been run already")]
    public async Task PopulateMatchesToCome()
    {
        // Read from main page with results
        // For each gameweek
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
        // Connect to db container 
        
        CreateQueueConnection(out _, out Container container);
        
        // Add matches to db
        foreach (var match in matches)
        {
            match.Id = Guid.NewGuid().ToString();
            var createItem = await container.CreateItemAsync(match);
        }
        
    }
    public async Task<IEnumerable<FootballTeamDTO?>> GetFootballTeams()
    {
        CreateDatabaseConnection(out _, out Container container);
        var dbContainerResponse = container.GetItemQueryIterator<FootballTeamDTO>(new QueryDefinition("SELECT * from c"));
        List<FootballTeamDTO> list = new();
        while (dbContainerResponse.HasMoreResults)
        {
            FeedResponse<FootballTeamDTO> response = await dbContainerResponse.ReadNextAsync();
            foreach (FootballTeamDTO team in response)
            {
                list.Add(team);
            }
        }
        return list;
    }

    public async Task<FootballTeamDTO?> GetFootballTeamById(string id)
    {
        CreateDatabaseConnection(out _, out Container container);
        IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
        var matches = queryable
        .Where(fb => fb.Id!.Equals(id));
        using FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
            return _mapper.Map<FootballTeamDTO>(response.FirstOrDefault());
        }
        return null;
    }

    public async Task<FootballTeamDTO?> GetFootballTeamByName(string name)
    {
        CreateDatabaseConnection(out _, out Container container);
        IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
        var matches = queryable
        .Where(fb => fb.Name!.Equals(name));
        using FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
            return _mapper.Map<FootballTeamDTO>(response.FirstOrDefault());
        }
        return null;
    }
    
    public async Task<FootballTeam?> GetTeamByName(string name)
    {
        CreateDatabaseConnection(out _, out Container container);
        IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
        var matches = queryable
            .Where(fb => fb.Name!.Equals(name));
        using FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
            return response.FirstOrDefault();
        }
        return null;
    }

    public async Task<FootballTeam?> AddTeam(FootballTeam ft)
    {
        CreateDatabaseConnection(out _, out Container container);
        ft.Id = Guid.NewGuid().ToString();
        var createTeam = await container.CreateItemAsync(ft);
        return createTeam;
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

    public async Task<FootballTeam?> UpdateFootballTeam(string id, FootballTeam footballTeam)
    {
        CreateDatabaseConnection(out _, out Container container);
        IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
        var matches = queryable
        .Where(fb => fb.Id!.Equals(id));
        using FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
            var team = response.FirstOrDefault();
            team = new FootballTeam
            {
                Id = footballTeam.Id,
                Name = footballTeam.Name,
                MatchesWon = footballTeam.MatchesWon,
                MatchesLost = footballTeam.MatchesLost,
                MatchesDraw = footballTeam.MatchesDraw,
                Description = footballTeam.Description,
                GoalsScored = footballTeam.GoalsScored,
                GoalsLost = footballTeam.GoalsLost,
                MatchesPlayed = footballTeam.MatchesPlayed
            };
            team.Points = CalculatePoints(team);
            team.GoalDifference = team.GoalsScored - team.GoalsLost;
            await container.UpsertItemAsync(team);
            return team;
        }
        return null;
    }
    
    public async Task<FootballTeam?> AddFootballTeam(FootballTeamDTO footballTeam)
    {
        CreateDatabaseConnection(out _, out Container container);
        var mappedTeam = _mapper.Map<FootballTeam>(footballTeam);
        mappedTeam.Id = Guid.NewGuid().ToString();
        mappedTeam.Points = CalculatePoints(mappedTeam);
        var createTeam = await container.CreateItemAsync(mappedTeam);
        return createTeam;
    }

    public async Task<FootballTeam?> DeleteFootballTeamById(string id)
    {
        CreateDatabaseConnection(out _, out Container container);
        IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
        var matches = queryable
        .Where(fb => fb.Id!.Equals(id));
        using FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
            var team = response.FirstOrDefault();
            await container.DeleteItemAsync<FootballTeam>(team!.Id, new PartitionKey(team!.Id));
            return team;
        }
        return null;
    }

    public async Task<FootballTeam?> DeleteFootballTeamByName(string name)
    {
        CreateDatabaseConnection(out _, out Container container);
        IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
        var matches = queryable
        .Where(fb => fb.Name!.Equals(name));
        using FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
            var team = response.FirstOrDefault();
            await container.DeleteItemAsync<FootballTeam>(team!.Id, new PartitionKey(team!.Id));
            return team;
        }
        return null;
    }
    
    public async Task<FootballTeam?> DeleteMultipleFootballTeamsByName(string name)
    {
        // Delete all times with given name
        
        CreateDatabaseConnection(out _, out Container container);
        IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
        var matches = queryable
            .Where(fb => fb.Name!.Equals(name));
        
        FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
            foreach (var team in response.Take(10))
            {
                await container.DeleteItemAsync<FootballTeam>(team!.Id, new PartitionKey(team!.Id));
            }
        }
        return null;
    }
    
    public void PopulateTeams()
    {
        // Nothing
    }

    [Obsolete("Not needed after initial population of db")]
    public async Task PopulateMatches()
    {
        // Create container
        CreateContainerMatches(out _, out Container container);

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
            record.Id = Guid.NewGuid().ToString();
            await container.CreateItemAsync(record);
        }
        
    }

    public async Task<ActionResult<string>> PredictResult(string team1, string team2)
    {
        List<MatchTeam> requestData = new();
        CreateContainerMatches(out _, out Container container);
        QueryDefinition query = new(@"select * from c where c.HomeTeam = '@team1' or c.AwayTeam = '@team1' and c.Date < GetCurrentDateTime() order by c.Date desc offset 0 limit 5"
            .Replace("@team1", team1));
        var dbContainerResponse = container.GetItemQueryIterator<FootballMatch>(query);
        while (dbContainerResponse.HasMoreResults)
        {
            FeedResponse<FootballMatch> response = await dbContainerResponse.ReadNextAsync();
            foreach (var match in response)
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
        }
        query = new(@"select * from c where c.HomeTeam = '@team2' or c.AwayTeam = '@team2' and c.Date < GetCurrentDateTime() order by c.Date desc offset 0 limit 5"
            .Replace("@team2", team2));
        dbContainerResponse = container.GetItemQueryIterator<FootballMatch>(query);
        while (dbContainerResponse.HasMoreResults)
        {
            FeedResponse<FootballMatch> response = await dbContainerResponse.ReadNextAsync();
            foreach (var match in response)
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
        var keyVaultEndpoint = new Uri(_configuration.GetConnectionString("VaultUriPred")!);
        var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
        var url = secretClient.GetSecretAsync("prediction-endpoint-url").Result.Value.Value;
        var apiKey = secretClient.GetSecretAsync("prediction-endpoint-api-key").Result.Value.Value;

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("A key should be provided to invoke the endpoint");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        client.BaseAddress = new Uri(url);

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

    public int CalculatePoints(FootballTeam footballTeam)
    {
        return (footballTeam.MatchesWon * 3) + footballTeam.MatchesDraw;
    }
    [Obsolete("Not needed after update")]
    public bool FootballTeamTableExists()
    {
        return _context.Teams != null;
    }

    private void CreateDatabaseConnection(out CosmosClient client, out Container container)
    {
        var keyVaultEndpoint = new Uri(_configuration.GetConnectionString("VaultUri")!);
        var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
        var accountEndpoint = secretClient.GetSecretAsync("CosmosDBEndpoint").Result.Value.Value;
        var accountKey = secretClient.GetSecretAsync("CosmosDBKey").Result.Value.Value;
        var dbName = secretClient.GetSecretAsync("DatabaseName").Result.Value.Value;
        client = new
        (
            accountEndpoint: accountEndpoint,
            authKeyOrResourceToken: accountKey!
        );
        container = client.GetContainer(dbName, containerName);
    }
    
    private void CreateQueueConnection(out CosmosClient client, out Container container)
    {
        var keyVaultEndpoint = new Uri(_configuration.GetConnectionString("VaultUri")!);
        var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
        var accountEndpoint = secretClient.GetSecretAsync("queueURI").Result.Value.Value;
        var accountKey = secretClient.GetSecretAsync("queuePK").Result.Value.Value;
        var dbName = secretClient.GetSecretAsync("queueDBname").Result.Value.Value;
        client = new
        (
            accountEndpoint: accountEndpoint,
            authKeyOrResourceToken: accountKey!
        );
        container = client.GetContainer(dbName, queueContainer);
    }

    private void CreateContainerMatches(out CosmosClient client, out Container container)
    {
        var keyVaultEndpoint = new Uri(_configuration.GetConnectionString("VaultUri")!);
        var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
        var accountEndpoint = secretClient.GetSecretAsync("CosmosDBEndpoint").Result.Value.Value;
        var accountKey = secretClient.GetSecretAsync("CosmosDBKey").Result.Value.Value;
        var dbName = secretClient.GetSecretAsync("DatabaseName").Result.Value.Value;
        client = new
        (
            accountEndpoint: accountEndpoint,
            authKeyOrResourceToken: accountKey!
        );
        Database db = client.GetDatabase(dbName);
        var throughput = ThroughputProperties.CreateAutoscaleThroughput(1000);

        var response = db.CreateContainerIfNotExistsAsync(new ContainerProperties(macthesContainer, "/id"), throughput);
        container = response.Result.Container;
    }
    
}
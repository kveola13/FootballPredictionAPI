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
using System.Net;
using System.Net.Http.Headers;
using CsvHelper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

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

    public async Task GetNewMatches()
    {
        // Connect to queue db
        // Read matches played before DateTime.Now
        // If many, select n first (played longest ago)
        
        CreateQueueConnection(out _, out Container container);
        QueryDefinition query = new QueryDefinition("select * from c where c.Date < NOW() order by c.Date limit 2");
        var dbContainerResponse = container.GetItemQueryIterator<Match>(query);
        List<Match> URIs = new List<Match>();
        while (dbContainerResponse.HasMoreResults)
        {
            FeedResponse<Match> response = await dbContainerResponse.ReadNextAsync();
            foreach (var match in response)
            {
                URIs.Add(match);
            }
        }
        
        // Read content 
        // Create Match objects
        // Update Teams
        // Add to db
    }
    [Obsolete("One time job & has been run already")]
    public async Task PopulateMatchesToCome()
    {
        Console.WriteLine("POPULATE MATCHESTOCOME _repository");
        // Read from main page with results
        // For each gameweek
        var matchDays = _webCrawler.GetMatchDays();
        Console.WriteLine(matchDays.Count);
        List<Match> matches = new List<Match>();
        foreach (var matchday in matchDays)
        {
            Console.WriteLine(matchday);
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
            Console.WriteLine(createItem);
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
                Id = id,
                Name = footballTeam.Name,
                MatchesWon = footballTeam.MatchesWon,
                MatchesLost = footballTeam.MatchesLost,
                MatchesDraw = footballTeam.MatchesDraw,
                Description = footballTeam.Description,
            };
            await container.UpsertItemAsync(team);
            return team;
        }
        return null;
    }
    
    [Obsolete("Not needed after teams are initialized")]
    public async Task<object> UpdateAllTeams()
    {
        // Update goalDifference when all teams are added / updated based on matches
        
        CreateDatabaseConnection(out _, out Container container);
        IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();

        using FeedIterator<FootballTeam> linqFeed = queryable.ToFeedIterator();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
            foreach (var team in response)
            {
                team.GoalDifference = team.GoalsScored - team.GoalsLost;
                await container.UpsertItemAsync(team);
            }
          
        }
        return null!;
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

    [Obsolete("This will no longer be needed after a CosmosDB integration")]
    public async Task<IEnumerable<FootballTeamDTO>> Seed()
    {
        string path = "Data/laliga21-22.csv";

        string[] lines = await File.ReadAllLinesAsync(path);
        var teamsData = lines.Skip(1);
        List<FootballTeam> teams = new();
        foreach (string line in teamsData)
        {
            string[] columns = line.Split(",");
            string ftName = columns[1];
            int wins = int.Parse(columns[3]);
            int lost = int.Parse(columns[5]);
            int draw = int.Parse(columns[4]);
            FootballTeam team = new()
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

        return _mapper.Map<IEnumerable<FootballTeamDTO>>(teams);

    }
    
    [Obsolete("This will no longer be needed after CosmosDB population")]
    public void PopulateTeams()
    {
        // TO DO: Check if container exists, create if not
        
        List<FootballTeam> teams = new List<FootballTeam>();
        IEnumerable<FootballMatch> records;
        using (var reader = new StreamReader("matches_teams_current.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            records = csv.GetRecords<FootballMatch>().ToList();
        }

        foreach (var record in records)
        {
            Console.WriteLine(record.Score);
            string HT = record.HomeTeam!;
            string AT = record.AwayTeam!;
            Console.WriteLine(HT + ", " + AT);
            FootballTeam ft = teams.FirstOrDefault(t => t.Name == HT)!;
            if (ft == null)
            {
                FootballTeam ftN = new FootballTeam
                {
                    Name = HT,
                    Points = 0,
                    MatchesWon = record.HTResult == "W" ? 1 : 0,
                    MatchesLost = record.HTResult == "L" ? 1 : 0,
                    MatchesDraw = record.HTResult == "D" ? 1 : 0,
                    Description = "",
                    GoalsScored = int.Parse(record.Score!.Split(":")[0]),
                    GoalsLost = int.Parse(record.Score.Split(":")[1]),
                    GoalDifference = 0,
                    MatchesPlayed = 1
                };
                ftN.Points = CalculatePoints(ftN);
                ftN.GoalDifference = ftN.GoalsScored - ftN.GoalsLost;
                teams.Add(ftN);
            }
            else
            {
                ft.GoalsScored += int.Parse(record.Score!.Split(":")[0]);
                ft.GoalsLost += int.Parse(record.Score.Split(":")[1]);
                ft.MatchesWon += record.HTResult == "W" ? 1 : 0;
                ft.MatchesLost += record.HTResult == "L" ? 1 : 0;
                ft.MatchesDraw += record.HTResult == "D" ? 1 : 0;
                ft.Points = CalculatePoints(ft);
            }

            
            FootballTeam aft = teams.FirstOrDefault(t => t.Name == AT)!;
            if (aft == null)
            {
                FootballTeam ftN = new FootballTeam
                {
                    Name = AT,
                    Points = 0,
                    MatchesWon = record.HTResult == "L" ? 1 : 0,
                    MatchesLost = record.HTResult == "L" ? 1 : 0,
                    MatchesDraw = record.HTResult == "W" ? 1 : 0,
                    Description = "",
                    GoalsScored = int.Parse(record.Score.Split(":")[1]),
                    GoalsLost = int.Parse(record.Score.Split(":")[0]),
                    GoalDifference = 0,
                    MatchesPlayed = 1
                };
                ftN.Points = CalculatePoints(ftN);
                ftN.GoalDifference = ftN.GoalsScored - ftN.GoalsLost;
                teams.Add(ftN);
                Console.WriteLine("Away team " + ftN.Points);
            }
            else
            {
                Console.WriteLine("AT before " + aft.Points);
                aft.GoalsScored += int.Parse(record.Score.Split(":")[1]);
                aft.GoalsLost += int.Parse(record.Score.Split(":")[0]);
                aft.MatchesWon += record.HTResult == "L" ? 1 : 0;
                aft.MatchesLost += record.HTResult == "W" ? 1 : 0;
                aft.MatchesDraw += record.HTResult == "D" ? 1 : 0;
                aft.Points = CalculatePoints(aft);
                Console.WriteLine("AT after " + aft.Points);
            }
        }
        foreach (var team in teams)
        {
            var postTeam = AddFootballTeam(team);
        }
        var updateTeam = UpdateAllTeams();
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

        Console.WriteLine(records.Count());
        foreach (var record in records)
        {
            
            // Here add to db
            record.Id = Guid.NewGuid().ToString();
            await container.CreateItemAsync(record);
        }
        
    }

    public async Task<ActionResult<string>> PredictResult(string team1, string team2)
    {
        var handler = new HttpClientHandler()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
        };
        using (var client = new HttpClient(handler))
        {
            // Request data goes here
            // The example below assumes JSON formatting which may be updated
            // depending on the format your endpoint expects.
            // More information can be found here:
            // https://docs.microsoft.com/azure/machine-learning/how-to-deploy-advanced-entry-script
            var requestBody = @"{
                  ""Inputs"": {
                    ""input1"": [
                      {
                        ""HomeTeam"": ""Sevilla"",
                        ""AwayTeam"": ""Barcelona""
                      }
                    ]
                  },
                  ""GlobalParameters"": {}
                }";
                
            // Replace this with the primary/secondary key or AMLToken for the endpoint
            var keyVaultEndpoint = new Uri(_configuration.GetConnectionString("VaultUriPred")!);
            var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
            var url = secretClient.GetSecretAsync("prediction-endpoint-url").Result.Value.Value;
            var apiKey = secretClient.GetSecretAsync("prediction-endpoint-api-key").Result.Value.Value;

            if (string.IsNullOrEmpty(apiKey))  
            {
                throw new Exception("A key should be provided to invoke the endpoint");
            }
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", apiKey);
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
            HttpResponseMessage response = await client.PostAsync("", content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                string predictions = String.Format("Result: {0}", result);
                Console.WriteLine(predictions);
                return predictions;
            }
            else
            {
                
                Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                // Print the headers - they include the requert ID and the timestamp,
                // which are useful for debugging the failure
                Console.WriteLine(response.Headers.ToString());

                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
                return string.Format("The request failed with status code: {0}", response.StatusCode);
            }
        }
    }

    public async Task<FootballTeam?> AddFootballTeam(FootballTeam footballTeam)
    {
        CreateDatabaseConnection(out _, out Container container);
        //var mappedTeam = _mapper.Map<FootballTeam>(footballTeam);
        footballTeam.Id = Guid.NewGuid().ToString();
        var createTeam = await container.CreateItemAsync(footballTeam);
        return createTeam;
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
        var keyVaultEndpoint = new Uri(_configuration.GetConnectionString("VaultUriPred")!);
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
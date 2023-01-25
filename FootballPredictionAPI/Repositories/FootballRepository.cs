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
using CsvHelper;

namespace FootballPredictionAPI.Repositories;

public class FootballRepository : IFootballRepository
{

    private readonly FootballTeamContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly string containerName = "teams";

    public FootballRepository(FootballTeamContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
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
    public async Task<object> PopulateTeams()
    {
        // Check if container exists, create if not
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
            string HT = record.HomeTeam;
            string AT = record.AwayTeam;
            Console.WriteLine(HT + ", " + AT);
            FootballTeam ft = teams.FirstOrDefault(t => t.Name == HT);
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
                    GoalsScored = int.Parse(record.Score.Split(":")[0]),
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
                ft.GoalsScored += int.Parse(record.Score.Split(":")[0]);
                ft.GoalsLost += int.Parse(record.Score.Split(":")[1]);
                ft.MatchesWon += record.HTResult == "W" ? 1 : 0;
                ft.MatchesLost += record.HTResult == "L" ? 1 : 0;
                ft.MatchesDraw += record.HTResult == "D" ? 1 : 0;
                ft.Points = CalculatePoints(ft);
            }

            
            FootballTeam aft = teams.FirstOrDefault(t => t.Name == AT);
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
        
        return null;
    }
    
    public async Task PopulateMatches()
    {
        // Create container
        //CreateDatabaseConnection(out _, out Container matchescontainer);
        
        // Add matches
        List<FootballMatch> matches = new List<FootballMatch>();
        IEnumerable<FootballMatch> records;
        using (var reader = new StreamReader("matches_teams_current.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            records = csv.GetRecords<FootballMatch>().ToList();
        }

        foreach (var record in records)
        {
            // Here add to db
            Console.WriteLine(record.HTResult);
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
        var keyVaultEndpoint = new Uri(_configuration["Keyvault:VaultUri"]!);
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
}
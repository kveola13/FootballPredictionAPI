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
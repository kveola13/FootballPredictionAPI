using AutoMapper;
using FootballPredictionAPI.Context;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;
using System.ComponentModel;

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
    ***REMOVED***
    public async Task<IEnumerable<FootballTeamDTO>> GetFootballTeams()
    {
        CreateDatabaseConnection(out _, out Microsoft.Azure.Cosmos.Container container);
        var dbContainerResponse = container.GetItemQueryIterator<FootballTeamDTO>(new QueryDefinition("SELECT * from c"));
        List<FootballTeamDTO> list = new();
        while (dbContainerResponse.HasMoreResults)
        {
            FeedResponse<FootballTeamDTO> response = await dbContainerResponse.ReadNextAsync();
            foreach (FootballTeamDTO team in response)
            {
                list.Add(team);
            ***REMOVED***
        ***REMOVED***
        return list;
    ***REMOVED***

    public async Task<FootballTeamDTO> GetFootballTeamById(string id)
    {
        CreateDatabaseConnection(out _, out Microsoft.Azure.Cosmos.Container container);
        var response = container.GetItemQueryIterator<FootballTeam>(new QueryDefinition("SELECT * from c where name like 'string'"));
        var test = response.ReadNextAsync().Result.AsQueryable();
        var team = await test.ToListAsync();
        return _mapper.Map<FootballTeamDTO>(team);
    ***REMOVED***

    public async Task<FootballTeamDTO> GetFootballTeamByName(string name)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(ft => ft.Name!.ToLower().Equals(name.ToLower()));
        return _mapper.Map<FootballTeamDTO>(team);
    ***REMOVED***

    public async Task<bool> UpdateFootballTeam(string id, FootballTeam footballTeam)
    {
        footballTeam.Points = CalculatePoints(footballTeam);
        _context.Teams.Update(footballTeam);
        int result = await _context.SaveChangesAsync();
        return result > 0;
    ***REMOVED***

    public async Task<bool> DeleteFootballTeamById(string id)
    {
        _context.Teams.Remove(_context.Teams.FirstOrDefault(ft => ft.Id!.Equals(id)));
        int result = await _context.SaveChangesAsync();
        return result > 0;
    ***REMOVED***

    public async Task<bool> DeleteFootballTeamByName(string name)
    {
        _context.Teams.Remove(_context.Teams.FirstOrDefault(ft => ft.Name.ToLower().Equals(name.ToLower())));
        int result = await _context.SaveChangesAsync();
        return result > 0;
    ***REMOVED***

    public async Task<bool> AddFootballTeam(FootballTeam footballTeam)
    {
        await _context.Teams.AddAsync(footballTeam);
        int result = await _context.SaveChangesAsync();
        return result > 0;
    ***REMOVED***

    public async Task<IEnumerable<FootballTeamDTO>> Seed()
    {
        string path = "Data/laliga21-22.csv";

        string[] lines = await System.IO.File.ReadAllLinesAsync(path);
        var teamsData = lines.Skip(1);
        List<FootballTeam> teams = new List<FootballTeam>();
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
                Description = $"Team located in Spain: {ftName***REMOVED***"
            ***REMOVED***;
            team.Points = CalculatePoints(team);
            teams.Add(team);
            await _context.Teams.AddAsync(team);
        ***REMOVED***

        await _context.SaveChangesAsync();

        return _mapper.Map<IEnumerable<FootballTeamDTO>>(teams);

    ***REMOVED***

    public int CalculatePoints(FootballTeam footballTeam)
    {
        return (footballTeam.MatchesWon * 3) + footballTeam.MatchesDraw;
    ***REMOVED***

    public bool ListEmpty()
    {
        return !_context.Teams.Any();
    ***REMOVED***

    public bool FootballTeamTableExists()
    {
        return _context.Teams != null;
    ***REMOVED***

    private void CreateDatabaseConnection(out CosmosClient client, out Microsoft.Azure.Cosmos.Container container)
    {
        var dbName = _configuration["ConnectionStrings:DATABASE_NAME"]!;
        var accountEndpoint = _configuration["ConnectionStrings:COSMOS_ENDPOINT"]!;
        var accountKey = _configuration["ConnectionStrings:COSMOS_KEY"]!;
        client = new
        (
            accountEndpoint: accountEndpoint,
            authKeyOrResourceToken: accountKey!
        );
        container = client.GetContainer(dbName, containerName);
    ***REMOVED***
***REMOVED***
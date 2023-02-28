using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FootballPredictionAPI.Context;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos;
using System.Globalization;
using System.Net.Http.Headers;

namespace FootballPredictionAPI.Repositories
{
    public class AdminFootballRepository : IAdminFootballRepository
    {
        private readonly FootballTeamContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly string containerName = "teams";

        public AdminFootballRepository(FootballTeamContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        ***REMOVED***
        public async Task<IEnumerable<FootballTeam?>> GetFootballTeams()
        {
            CreateDatabaseConnection(out _, out Container container);
            var dbContainerResponse = container.GetItemQueryIterator<FootballTeam>(new QueryDefinition("SELECT * from c"));
            List<FootballTeam> list = new();
            while (dbContainerResponse.HasMoreResults)
            {
                FeedResponse<FootballTeam> response = await dbContainerResponse.ReadNextAsync();
                foreach (FootballTeam team in response)
                {
                    list.Add(team);
                ***REMOVED***
            ***REMOVED***
            return list!;
        ***REMOVED***

        public async Task<FootballTeam?> GetFootballTeamById(string id)
        {
            CreateDatabaseConnection(out _, out Container container);
            IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
            var matches = queryable
            .Where(fb => fb.id!.Equals(id));
            using FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
                return _mapper.Map<FootballTeam>(response.FirstOrDefault());
            ***REMOVED***
            return null;
        ***REMOVED***

        public async Task<FootballTeam?> GetFootballTeamByName(string name)
        {
            CreateDatabaseConnection(out _, out Container container);
            IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
            var matches = queryable
            .Where(fb => fb.Name!.Equals(name));
            using FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
                return _mapper.Map<FootballTeam>(response.FirstOrDefault());
            ***REMOVED***
            return null;
        ***REMOVED***

        public async Task<FootballTeam?> UpdateFootballTeam(string id, FootballTeam footballTeam)
        {
            CreateDatabaseConnection(out _, out Container container);
            IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
            var matches = queryable
            .Where(fb => fb.id!.Equals(id));
            using FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
                var team = response.FirstOrDefault();
                team = new FootballTeam
                {
                    id = id,
                    Name = footballTeam.Name,
                    MatchesWon = footballTeam.MatchesWon,
                    MatchesLost = footballTeam.MatchesLost,
                    MatchesDraw = footballTeam.MatchesDraw,
                    Description = footballTeam.Description,
                ***REMOVED***;
                await container.UpsertItemAsync(team);
                return team;
            ***REMOVED***
            return null;
        ***REMOVED***

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
                ***REMOVED***
            ***REMOVED***
            return null!;
        ***REMOVED***

        public async Task<FootballTeam?> AddFootballTeam(FootballTeam footballTeam)
        {
            CreateDatabaseConnection(out _, out Container container);
            var mappedTeam = _mapper.Map<FootballTeam>(footballTeam);
            mappedTeam.id = Guid.NewGuid().ToString();
            mappedTeam.Points = CalculatePoints(mappedTeam);
            var createTeam = await container.CreateItemAsync(mappedTeam);
            return createTeam;
        ***REMOVED***

        public async Task<FootballTeam?> DeleteFootballTeamById(string id)
        {
            CreateDatabaseConnection(out _, out Container container);
            IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
            var matches = queryable
            .Where(fb => fb.id!.Equals(id));
            using FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
                var team = response.FirstOrDefault();
                await container.DeleteItemAsync<FootballTeam>(team!.id, new PartitionKey(team!.id));
                return team;
            ***REMOVED***
            return null;
        ***REMOVED***

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
                await container.DeleteItemAsync<FootballTeam>(team!.id, new PartitionKey(team!.id));
                return team;
            ***REMOVED***
            return null;
        ***REMOVED***

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
                    await container.DeleteItemAsync<FootballTeam>(team!.id, new PartitionKey(team!.id));
                ***REMOVED***
            ***REMOVED***
            return null;
        ***REMOVED***

        [Obsolete("This will no longer be needed after a CosmosDB integration")]
        public async Task<IEnumerable<FootballTeam>> Seed()
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
                    Description = $"Team located in Spain: {ftName***REMOVED***"
                ***REMOVED***;
                team.Points = CalculatePoints(team);
                teams.Add(team);
                await _context.Teams.AddAsync(team);
            ***REMOVED***

            await _context.SaveChangesAsync();

            return _mapper.Map<IEnumerable<FootballTeam>>(teams);
        ***REMOVED***

        public async Task<ActionResult<string>> PredictResult(string team1, string team2)
        {
            var handler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) => { return true; ***REMOVED***
            ***REMOVED***;
            using var client = new HttpClient(handler);
            var requestBody = @"{
                  ""Inputs"": {
                    ""input1"": [
                      {
                        ""HomeTeam"": ""Sevilla"",
                        ""AwayTeam"": ""Barcelona""
                      ***REMOVED***
                    ]
                  ***REMOVED***,
                  ""GlobalParameters"": {***REMOVED***
                ***REMOVED***";

            var keyVaultEndpoint = new Uri(_configuration.GetConnectionString("VaultUriPred")!);
            var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
            var url = secretClient.GetSecretAsync("prediction-endpoint-url").Result.Value.Value;
            var apiKey = secretClient.GetSecretAsync("prediction-endpoint-api-key").Result.Value.Value;

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("A key should be provided to invoke the endpoint");
            ***REMOVED***

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.BaseAddress = new Uri(url);

            var content = new StringContent(requestBody);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync("", content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                string predictions = String.Format("Result: {0***REMOVED***", result);
                Console.WriteLine(predictions);
                return predictions;
            ***REMOVED***
            else
            {

                Console.WriteLine(string.Format("The request failed with status code: {0***REMOVED***", response.StatusCode));

                Console.WriteLine(response.Headers.ToString());

                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
                return string.Format("The request failed with status code: {0***REMOVED***", response.StatusCode);
            ***REMOVED***
        ***REMOVED***

        public int CalculatePoints(FootballTeam footballTeam)
        {
            return (footballTeam.MatchesWon * 3) + footballTeam.MatchesDraw;
        ***REMOVED***
        [Obsolete("Not needed after update")]
        public bool FootballTeamTableExists()
        {
            return _context.Teams != null;
        ***REMOVED***

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
        ***REMOVED***
    ***REMOVED***
***REMOVED***

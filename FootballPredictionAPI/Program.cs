using FootballPredictionAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using AutoMapper;
using FootballPredictionAPI;
using FootballPredictionAPI.Context;
using Microsoft.Azure.Cosmos;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using FootballPredictionAPI.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbName = builder.Configuration["ConnectionStrings:DATABASE_NAME"]!;
var accountEndpoint = builder.Configuration.GetConnectionString("COSMOS_ENDPOINT")!;
var accountKey = builder.Configuration.GetConnectionString("COSMOS_KEY")!;
var containerName = "teams";
ContainerProperties containerProperties = new(
    id: containerName,
    partitionKeyPath: "/id"
);

builder.Services.AddControllers();
using CosmosClient client = new
(
    accountEndpoint: accountEndpoint,
    authKeyOrResourceToken: accountKey!
);
Database database = await client.CreateDatabaseIfNotExistsAsync(id: dbName);
Console.WriteLine($"Created a new database {database.Id}");
Console.WriteLine($"Getting container: {database.GetContainer(containerName).Id}");
ContainerResponse containerResponse = await database.CreateContainerIfNotExistsAsync(containerProperties, ThroughputProperties.CreateAutoscaleThroughput(1000));

builder.Services.AddDbContext<FootballTeamContext>(optionsAction => optionsAction.UseCosmos(accountEndpoint!, accountKey!, dbName!));
var dbContainerResponse = database.GetContainer(containerName).GetItemQueryIterator<FootballTeamDTO>(new QueryDefinition("SELECT * from c"));
while (dbContainerResponse.HasMoreResults)
{
    FeedResponse<FootballTeamDTO> response = await dbContainerResponse.ReadNextAsync();

    // Iterate query results
    foreach (FootballTeamDTO team in response)
    {
        Console.WriteLine($"Found item:\t{team.Name}");
    }
}
//builder.Services.AddDbContext<FootballTeamContext>(opt => opt.UseInMemoryDatabase(dbName));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

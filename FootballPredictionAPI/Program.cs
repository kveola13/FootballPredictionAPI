using FootballPredictionAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using AutoMapper;
using FootballPredictionAPI;
using FootballPredictionAPI.Context;
using Microsoft.Azure.Cosmos;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbName = "FootballTeams";
var accountEndpoint = builder.Configuration.GetConnectionString("COSMOS_ENDPOINT")!;
var accountKey = builder.Configuration.GetConnectionString("COSMOS_KEY")!;

builder.Services.AddControllers();
using CosmosClient client = new
(
    accountEndpoint: accountEndpoint,
    authKeyOrResourceToken: accountKey!
);
Database database = client.GetDatabase(id: dbName);
Console.WriteLine($"Created a new database {database.Id}");
Console.WriteLine($"Getting container: {database.GetContainer("Teams").Id}");
Container container = database.GetContainer(id: dbName);
//builder.Services.AddDbContext<FootballTeamContext>(optionsAction => optionsAction.UseCosmos(accountEndpoint!, accountKey!, dbName));
builder.Services.AddDbContext<FootballTeamContext>(opt => opt.UseInMemoryDatabase(dbName));
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

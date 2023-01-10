using FootballPredictionAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using AutoMapper;
using FootballPredictionAPI;
using FootballPredictionAPI.Context;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbName = "FootballTeams";
builder.Services.AddControllers();
using CosmosClient client = new(
    accountEndpoint: builder.Configuration.GetConnectionString("COSMOS_ENDPOINT")!, 
    authKeyOrResourceToken: builder.Configuration.GetConnectionString("COSMOS_KEY")!
    );
//builder.Services.AddDbContext<FootballTeamContext>(optionsAction => optionsAction.UseCosmos(connection, dbName));
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

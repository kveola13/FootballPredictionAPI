using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FootballPredictionAPI;
using FootballPredictionAPI.Context;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Repositories;
using Microsoft.Azure.KeyVault;
using Microsoft.AspNetCore.Authentication;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

var dbName = builder.Configuration["ConnectionStrings:DATABASE_NAME"]!;
var accountEndpoint = builder.Configuration.GetConnectionString("COSMOS_ENDPOINT")!;
var accountKey = builder.Configuration.GetConnectionString("COSMOS_KEY")!;
builder.Services.AddControllers();
builder.Services.AddDbContext<FootballTeamContext>(optionsAction => optionsAction.UseCosmos(accountEndpoint!, accountKey!, dbName!));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddScoped<IFootballRepository, FootballRepository>();
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

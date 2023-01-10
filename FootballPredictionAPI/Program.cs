using FootballPredictionAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using AutoMapper;
using FootballPredictionAPI;
using FootballPredictionAPI.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<FootballTeamContext>(opt => opt.UseInMemoryDatabase("FootballTeams"));
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
***REMOVED***

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

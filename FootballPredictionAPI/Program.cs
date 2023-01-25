using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FootballPredictionAPI;
using FootballPredictionAPI.Context;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Repositories;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(builder.Configuration["Keyvault:VaultUri"]!);
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
var client = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
var accountEndpoint = client.GetSecretAsync("CosmosDBEndpoint").Result.Value.Value;
var accountKey = client.GetSecretAsync("CosmosDBKey").Result.Value.Value;
var dbName = client.GetSecretAsync("DatabaseName").Result.Value.Value;
var containerName = "teams";


var CosmosClient = new CosmosClient(client.GetSecretAsync("ConnectionStrings").Result.Value.Value, 
    new CosmosClientOptions() { ***REMOVED*** )
    .CreateDatabaseIfNotExistsAsync(dbName);

CosmosClient.Result.Database.CreateContainerIfNotExistsAsync(new ContainerProperties() { PartitionKeyPath="/id", Id=containerName ***REMOVED***);

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
***REMOVED***

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

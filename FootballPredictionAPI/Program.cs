using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FootballPredictionAPI;
using FootballPredictionAPI.Context;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Repositories;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);
var keyvaultUri = builder.Configuration.GetConnectionString("VaultUri")!;
var keyVaultEndpoint = new Uri(keyvaultUri)!;
var client = new SecretClient(keyVaultEndpoint!, new DefaultAzureCredential());
StringConstrains.APIConnectionString = client.GetSecretAsync("ConnectionStrings").Result.Value.Value;
StringConstrains.QueueConnectionString = client.GetSecretAsync("queueConnectionString").Result.Value.Value;
StringConstrains.DatabaseName = client.GetSecretAsync("DatabaseName").Result.Value.Value;
StringConstrains.QueueDBName = client.GetSecretAsync("queueDBname").Result.Value.Value;
StringConstrains.PredictionUrl = client.GetSecretAsync("prediction-endpoint-url").Result.Value.Value;
StringConstrains.PredictionAPIKey = client.GetSecretAsync("prediction-endpoint-api-key").Result.Value.Value;
var containerName = "teams";



builder.Services.AddDbContext<FootballTeamContext>(options => options.UseCosmos(StringConstrains.APIConnectionString, StringConstrains.DatabaseName));
builder.Services.AddDbContext<MatchQueueContext>(options => options.UseCosmos(StringConstrains.QueueConnectionString ,StringConstrains.QueueDBName));
var CosmosClient = new CosmosClient(StringConstrains.APIConnectionString, new CosmosClientOptions() { } ).CreateDatabaseIfNotExistsAsync(StringConstrains.DatabaseName);

CosmosClient.Result.Database.CreateContainerIfNotExistsAsync(new ContainerProperties() { PartitionKeyPath="/id", Id=containerName });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IMapper? mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddScoped<IFootballRepository, FootballRepository>();
builder.Services.AddScoped<IAdminFootballRepository, AdminFootballRepository>();
builder.Services.AddScoped<IFootballCosmosRepository, FootballCosmosRepository>();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

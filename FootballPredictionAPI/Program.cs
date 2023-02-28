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
var accountEndpoint = client.GetSecretAsync("CosmosDBEndpoint").Result.Value.Value;
var accountKey = client.GetSecretAsync("CosmosDBKey").Result.Value.Value;
var APIdbName = client.GetSecretAsync("DatabaseName").Result.Value.Value;
var containerName = "teams";

var queueDBName = "***REMOVED***";


var queueConnectionString =
***REMOVED***


var APIConnectionString =
***REMOVED***

builder.Services.AddDbContext<FootballTeamContext>(options => options.UseCosmos(APIConnectionString, APIdbName));
builder.Services.AddDbContext<MatchQueueContext>(options => options.UseCosmos(queueConnectionString ,queueDBName));
var CosmosClient = new CosmosClient(client.GetSecretAsync("ConnectionStrings").Result.Value.Value, 
    new CosmosClientOptions() { ***REMOVED*** )
    .CreateDatabaseIfNotExistsAsync(APIdbName);

CosmosClient.Result.Database.CreateContainerIfNotExistsAsync(new ContainerProperties() { PartitionKeyPath="/id", Id=containerName ***REMOVED***);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches();

builder.Services.AddControllers();
//builder.Services.AddDbContext<FootballTeamContext>(optionsAction => optionsAction.UseCosmos(accountEndpoint!, accountKey!, dbName!));
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
***REMOVED***

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

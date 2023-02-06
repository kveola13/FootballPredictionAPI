﻿using System.Net.Http.Headers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FootballPredictionAPI.Models;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;

namespace FootballPredictionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FootballTeamsController : ControllerBase
    {
        private readonly IFootballRepository _repository;
        private readonly IMapper _mapper;

        public FootballTeamsController(IMapper mapper, IFootballRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet("predict/{team1}/{team2}")]
        public async Task<ActionResult<string>> PredictResult(string team1, string team2)
        {
            // Get teams and check if exist
            var HomeTeam = _repository.GetFootballTeamByName(team1);
            var AwayTeam = _repository.GetFootballTeamByName(team2);
            if (HomeTeam == null || AwayTeam == null)
            {
                return NotFound("Team(s) not found!");
            }

            var handler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
            };
            using (var client = new HttpClient(handler))
            {
                // Request data goes here
                // The example below assumes JSON formatting which may be updated
                // depending on the format your endpoint expects.
                // More information can be found here:
                // https://docs.microsoft.com/azure/machine-learning/how-to-deploy-advanced-entry-script
                var requestBody = @"{
                  ""Inputs"": {
                    ""input1"": [
                      {
                        ""HomeTeam"": ""Osasuna"",
                        ""AwayTeam"": ""Sevilla""
                        }
                    ]
                  },
                  ""GlobalParameters"": {}
                }";
                
                // Replace this with the primary/secondary key or AMLToken for the endpoint
                const string apiKey = "";
                if (string.IsNullOrEmpty(apiKey))  
                {
                    throw new Exception("A key should be provided to invoke the endpoint");
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", apiKey);
                client.BaseAddress = new Uri("http://16f78484-a06a-4bb6-a64a-6f18e517996c.norwayeast.azurecontainer.io/score");

                var content = new StringContent(requestBody);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // WARNING: The 'await' statement below can result in a deadlock
                // if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false)
                // so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)
                HttpResponseMessage response = await client.PostAsync("", content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Result: {0}", result);
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp,
                    // which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
            }
            return "";
        }

        [Obsolete("This will no longer be needed after a CosmosDB integration")]
        [HttpPost("seed")]
        public async Task<ActionResult<IEnumerable<FootballTeamDTO>>> SeedFootballTeam()
        {
            //Added a failsafe to this before confirmed removal.
            var teamExists = true;
            if (!teamExists)
            {
                return Ok(await _repository.Seed());
            }

            return Ok("Seed already in!");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<FootballTeamDTO>>> GetTeams()
        {
            return Ok(await _repository.GetFootballTeams());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FootballTeamDTO>> GetFootballTeam(string id)
        {
            var team = _repository.GetFootballTeamById(id);
            if (team.Result == null)
            {
                return NotFound("No team with that id is in the list.");
            }
            return Ok(await team!);
        }

        [HttpGet("getbyname/{name}")]
        public async Task<ActionResult<FootballTeamDTO>> GetFootballTeamByName(string name)
        {
            var team = _repository.GetFootballTeamByName(name);
            if (team.Result == null)
            {
                return NotFound("No team with that id is in the list.");
            }
            return Ok(await team!);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FootballTeamDTO>> PutFootballTeam(string id, FootballTeamDTO footballTeam)
        {
            var teamToUpdate = await _repository.GetFootballTeamById(id);
            if (teamToUpdate == null)
            {
                return NotFound("No team with that id found");
            }

            FootballTeam teamToChange = _mapper.Map<FootballTeam>(footballTeam);
            teamToChange.Id = id;
            return Ok(_repository.UpdateFootballTeam(id, teamToChange));
        }


        [HttpPost]
        public async Task<ActionResult<FootballTeam>> PostFootballTeam(FootballTeamDTO footballTeam)
        {
            if (_repository.GetFootballTeamByName(footballTeam.Name!).Result != null)
            {
                return Problem("A team with that name is already in the list!");
            }
            var postedTeam = await _repository.AddFootballTeam(footballTeam);
            return Ok(_mapper.Map<FootballTeamDTO>(postedTeam));
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<FootballTeam>> DeleteFootballTeam(string id)
        {
            if (await _repository.GetFootballTeamById(id) == null)
            {
                return NotFound("No team with that id in the list");
            }

            return Ok(await _repository.DeleteFootballTeamById(id));
        }

        [HttpDelete("deletebyname/{name}")]
        public async Task<ActionResult<FootballTeam>> DeleteFootballTeamByName(string name)
        {
            if (await _repository.GetFootballTeamByName(name) == null)
            {
                return NotFound("No team with that name in the list");
            }

            return Ok(await _repository.DeleteFootballTeamByName(name));
        }

        [Obsolete("Not needed after population is done")]
        [HttpPost("populateteams")]
        public async Task PopulateTeams()
        {
            await _repository.PopulateTeams();
        }
        
        [Obsolete("Not needed after initial population of db")]

        [HttpPost("populatematches")]
        public async Task PopulateMatches()
        {
            await _repository.PopulateMatches();
        }
        
    }
}

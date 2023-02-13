﻿using System.Net.Http.Headers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FootballPredictionAPI.Models;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using Microsoft.CodeAnalysis.Differencing;

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

        [HttpGet("getnewmatches")]
        public async Task<ActionResult<IEnumerable<Match>>> GetNewMatches()
        {
            var newMatches = await _repository.GetNewMatches();
            foreach (var match in newMatches)
            {
                // Read stats 
                // Save to FootballMatch object
                // Add to db
                // Update teams
                // Remove from queue
            }

            return null;
        }
        
        [Obsolete("One time job & has been run already")]
        [HttpPost("populatematchestocome")]
        public async Task PopulateMatchesToCome()
        {
            Console.WriteLine("Controller populate");
            await _repository.PopulateMatchesToCome();
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

            
            return await _repository.PredictResult(team1, team2);
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
        public void PopulateTeams()
        {
            _repository.PopulateTeams();
        }
        
        [Obsolete("Not needed after initial population of db")]

        [HttpPost("populatematches")]
        public async Task PopulateMatches()
        {
            await _repository.PopulateMatches();
        }
        
    }
}

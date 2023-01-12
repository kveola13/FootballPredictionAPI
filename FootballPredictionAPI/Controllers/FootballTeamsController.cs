using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballPredictionAPI.Models;
using FootballPredictionAPI.Context;
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

        [HttpPost("seed")]
        public async Task<ActionResult<FootballTeamDTO>> SeedFootballTeam()
        {
            var teamExists = _repository.Exists<string>("fc barcelona").Result;
            if (!teamExists)
            {
                return await _repository.Seed();
            }

            return Ok("Team already in!");
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FootballTeamDTO>>> GetTeams()
        {
            return Ok(await _repository.GetFootballTeams());
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<FootballTeamDTO>> GetFootballTeam(int id)
        {
            if (_repository.ListEmpty())
            {
                return NotFound("There are no teams in the list.");
            }
            if (_repository.Exists<int>(id).Result == false)
            {
                return NotFound("No team with that id is in the list.");
            }
            return await _repository.GetFootballTeamById(id);
        }
        [HttpGet("getbyname/{name}")]
        public async Task<ActionResult<FootballTeamDTO>> GetFootballTeam(string name)
        {
            if (_repository.ListEmpty())
            {
                return NotFound("There are no teams in the list.");
            }
            if (_repository.Exists<string>(name).Result == false)
            {
                return NotFound("No team with that name is in the list.");
            }
            return Ok(await _repository.GetFootballTeamByName(name));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFootballTeam(int id, CreateFootballTeamDTO footballTeam)
        {
            if (!await _repository.Exists<int>(id))
            {
                return NotFound("No team with that name found");
            }

            FootballTeam teamToChange = _mapper.Map<FootballTeam>(footballTeam);
            teamToChange.Id = id;
            bool success = _repository.UpdateFootballTeam(id, teamToChange).Result;

            return success ? Ok("Changes has been made successfully") : Problem("Problem when trying to update the team in the database. Error.") ;
        }

        [HttpPost]
        public async Task<ActionResult<FootballTeam>>  PostFootballTeam(CreateFootballTeamDTO footballTeam)
        {
          if (!_repository.FootballTeamTableExists())
          {
              return Problem("Entity set 'FootballTeamContext.Teams'  is null.");
          }

          if (_repository.Exists<string>(footballTeam.Name).Result)
          {
              return Problem("A team with that name is already in the list!");
          }

          FootballTeam teamToAdd = _mapper.Map<FootballTeam>(footballTeam);
          teamToAdd.Points = _repository.CalculatePoints(teamToAdd);
          bool success = await _repository.AddFootballTeam(teamToAdd);
          return success ? Ok($"{footballTeam.Name} has been added to the list") : Problem("Problem when trying to add the team to the database. Error.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFootballTeam(int id)
        {
            if (!await _repository.Exists<int>(id))
            {
                return NotFound("No team with that id in the list");
            }

            bool success = await _repository.DeleteFootballTeamById(id);
            return success ? Ok($"The team has been removed") : Problem("Problem when trying to remove the team from the database. Error.");
        }
        
        [HttpDelete("deletebyname/{name}")]
        public async Task<IActionResult> DeleteFootballTeam(string name)
        {
            if (!await _repository.Exists<string>(name))
            {
                return NotFound("No team with that name in the list");
            }

            bool success = await _repository.DeleteFootballTeamByName(name);
            return success ? Ok($"The team has been removed") : Problem("Problem when trying to remove the team from the database. Error.");
        }
    }
}

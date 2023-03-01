using AutoMapper;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballPredictionAPI.Controllers
{
    [Route("api/admin/FootballTeams")]
    [ApiController]
    public class AdminFootballTeamsController : ControllerBase
    {
        private readonly IAdminFootballRepository _repository;
        private readonly IMapper _mapper;

        public AdminFootballTeamsController(IMapper mapper, IAdminFootballRepository repository)
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


            return await _repository.PredictResult(team1, team2);
        }

        [Obsolete("This is no longer be needed")]
        [HttpPost("seed")]
        public async Task<ActionResult<IEnumerable<FootballTeamDTO>>> SeedFootballTeam()
        {
            var teamExists = true;
            if (!teamExists)
            {
                return Ok(await _repository.Seed());
            }

            return Ok("Seed already in!");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<FootballTeam>>> GetTeams()
        {
            return Ok(await _repository.GetFootballTeams());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FootballTeam>> GetFootballTeam(string id)
        {
            var team = _repository.GetFootballTeamById(id);
            if (team.Result == null)
            {
                return NotFound("No team with that id is in the list.");
            }
            return Ok(await team!);
        }

        [HttpGet("getbyname/{name}")]
        public async Task<ActionResult<FootballTeam>> GetFootballTeamByName(string name)
        {
            var team = _repository.GetFootballTeamByName(name);
            if (team.Result == null)
            {
                return NotFound("No team with that id is in the list.");
            }
            return Ok(await team!);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FootballTeam>> PutFootballTeam(string id, FootballTeam footballTeam)
        {
            var teamToUpdate = await _repository.GetFootballTeamById(id);
            if (teamToUpdate == null)
            {
                return NotFound("No team with that id found");
            }

            FootballTeam teamToChange = _mapper.Map<FootballTeam>(footballTeam);
            teamToChange.id = id;
            return Ok(_repository.UpdateFootballTeam(id, teamToChange));
        }


        [HttpPost]
        public async Task<ActionResult<FootballTeam>> PostFootballTeam(FootballTeam footballTeam)
        {
            if (_repository.GetFootballTeamByName(footballTeam.Name!).Result != null)
            {
                return Problem("A team with that name is already in the list!");
            }
            var postedTeam = await _repository.AddFootballTeam(footballTeam);
            return Ok(_mapper.Map<FootballTeam>(postedTeam));
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
    }
}

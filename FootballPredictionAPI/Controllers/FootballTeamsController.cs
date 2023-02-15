using System.Net.Http.Headers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FootballPredictionAPI.Models;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using Microsoft.Azure.Cosmos.Linq;
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
            List<FootballMatch> fmatches = new();
            var newMatches = await _repository.GetNewMatches();
            if (newMatches.Count() == 0)
            {
                return Ok("No matches to update");
            }
            foreach (var match in newMatches)
            {
                // Read stats 
                // Save to FootballMatch object
                // TO DO: Update time
                var footballMatchesWithStats = _repository.ReadStatsForMatch(match);
                fmatches.Add(footballMatchesWithStats);
                // Add to db
                // Check if match with that date and teams already exists
                FootballMatch? resultAddMatch = await _repository.AddFootballMatchWithStats(footballMatchesWithStats);
                if (resultAddMatch == null)
                {
                    return BadRequest("Football Match with stat not added!");
                }
            }
            
            // Update teams
            foreach (var m in fmatches)
            {
                var homeTeam = await _repository.GetTeamByName(m.HomeTeam!);
                var awayTeam = await _repository.GetTeamByName(m.AwayTeam!);
                if (homeTeam != null)
                {
                    FootballTeam? hft = _repository.UpdateHomeTeam(m, homeTeam);
                    var responseUpdateht = await _repository.UpdateFootballTeam(hft!.Id!, hft);
                    if (responseUpdateht == null)
                    {
                        return BadRequest("Problems while updating home team!");
                    }
                }
                else
                {
                    FootballTeam hft = new()
                    {
                        Name = m.HomeTeam,
                        MatchesWon = m.HTGoals > m.ATGoals ? 1 : 0,
                        MatchesLost = m.HTGoals < m.ATGoals ? 1 : 0,
                        MatchesDraw = m.HTGoals == m.ATGoals ? 1 : 0,
                        GoalsScored = (int)m.HTGoals,
                        GoalsLost = (int)m.ATGoals,
                        MatchesPlayed = 1
                    };
                    hft.Points = _repository.CalculatePoints(hft);
                    hft.GoalDifference = hft.GoalsScored - hft.GoalsLost;
                    var responseAddTeam = await _repository.AddTeam(hft);
                    if (responseAddTeam == null)
                    {
                        return BadRequest("Problems while adding home team!");
                    }
                }

                if (awayTeam != null)
                {
                    FootballTeam? aft = _repository.UpdateAwayTeam(m, awayTeam);
                    var responseUpdateAt = await _repository.UpdateFootballTeam(aft.Id!, aft);
                    if (responseUpdateAt == null)
                    {
                        return BadRequest("Problems while updating Away Team!");
                    }
                }
                else
                {
                    FootballTeam aft = new FootballTeam
                    {
                        Name = m.AwayTeam,
                        MatchesWon = m.ATGoals > m.HTGoals ? 1 : 0,
                        MatchesLost = m.ATGoals < m.HTGoals ? 1 : 0,
                        MatchesDraw = m.ATGoals == m.HTGoals ? 1 : 0,
                        GoalsScored = (int)m.ATGoals,
                        GoalsLost = (int)m.HTGoals,
                        MatchesPlayed = 1
                    };
                    aft.Points = _repository.CalculatePoints(aft);
                    aft.GoalDifference = aft.GoalsScored - aft.GoalsLost;
                    var responseAddTeam = await _repository.AddTeam(aft);
                    if (responseAddTeam == null)
                    {
                        return BadRequest("Problems while adding away team!");
                    }
                }
            }
                
            // Remove from queue
            var response = await _repository.DeleteFromQueue(newMatches);
            if (response.Count() == newMatches.Count())
            {
                return Ok(response);
            }
            return BadRequest("Something went wrong while deleting!" + response);
        }
        
        [Obsolete("One time job & has been run already")]
        [HttpPost("populatematchestocome")]
        public async Task PopulateMatchesToCome()
        {
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

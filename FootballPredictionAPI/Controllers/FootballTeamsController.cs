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
        ***REMOVED***

        [Obsolete("This will no longer be needed after a CosmosDB integration")]
        [HttpPost("seed")]
        public async Task<ActionResult<IEnumerable<FootballTeamDTO>>> SeedFootballTeam()
        {
            //Added a failsafe to this before confirmed removal.
            var teamExists = true;
            if (!teamExists)
            {
                return Ok(await _repository.Seed());
            ***REMOVED***

            return Ok("Seed already in!");
        ***REMOVED***


        [HttpGet]
        public async Task<ActionResult<IEnumerable<FootballTeamDTO>>> GetTeams()
        {
            return Ok(await _repository.GetFootballTeams());
        ***REMOVED***

        [HttpGet("{id***REMOVED***")]
        public async Task<ActionResult<FootballTeamDTO>> GetFootballTeam(string id)
        {
            var team = _repository.GetFootballTeamById(id);
            if (team.Result == null)
            {
                return NotFound("No team with that id is in the list.");
            ***REMOVED***
            return Ok(await team!);
        ***REMOVED***

        [HttpGet("getbyname/{name***REMOVED***")]
        public async Task<ActionResult<FootballTeamDTO>> GetFootballTeamByName(string name)
        {
            var team = _repository.GetFootballTeamByName(name);
            if (team.Result == null)
            {
                return NotFound("No team with that id is in the list.");
            ***REMOVED***
            return Ok(await team!);
        ***REMOVED***

        [HttpPut("{id***REMOVED***")]
        public async Task<IActionResult> PutFootballTeam(string id, FootballTeamDTO footballTeam)
        {
            var teamToUpdate = await _repository.GetFootballTeamById(id);
            if (teamToUpdate == null)
            {
                return NotFound("No team with that id found");
            ***REMOVED***

            FootballTeam teamToChange = _mapper.Map<FootballTeam>(footballTeam);
            teamToChange.Id = id;
            bool success = _repository.UpdateFootballTeam(id, teamToChange).Result;

            return success ? Ok("Changes has been made successfully") : Problem("Problem when trying to update the team in the database. Error.");
        ***REMOVED***


        [HttpPost]
        public async Task<ActionResult<FootballTeam>> PostFootballTeam(FootballTeamDTO footballTeam)
        {
            if (_repository.GetFootballTeamByName(footballTeam.Name!).Result != null)
            {
                return Problem("A team with that name is already in the list!");
            ***REMOVED***
            var postedTeam = await _repository.AddFootballTeam(footballTeam);
            return Ok(_mapper.Map<FootballTeamDTO>(postedTeam));
        ***REMOVED***


        [HttpDelete("{id***REMOVED***")]
        public async Task<IActionResult> DeleteFootballTeam(string id)
        {
            if (await _repository.GetFootballTeamById(id) == null)
            {
                return NotFound("No team with that id in the list");
            ***REMOVED***

            bool success = await _repository.DeleteFootballTeamById(id);
            return success ? Ok($"The team has been removed") : Problem("Problem when trying to remove the team from the database. Error.");
        ***REMOVED***

        [HttpDelete("deletebyname/{name***REMOVED***")]
        public async Task<IActionResult> DeleteFootballTeamByName(string name)
        {
            if (await _repository.GetFootballTeamByName(name) == null)
            {
                return NotFound("No team with that name in the list");
            ***REMOVED***

            bool success = await _repository.DeleteFootballTeamByName(name);
            return success ? Ok($"The team has been removed") : Problem("Problem when trying to remove the team from the database. Error.");
        ***REMOVED***
    ***REMOVED***
***REMOVED***

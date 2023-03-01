using System.Collections;
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
        private readonly IFootballCosmosRepository _cosmosRepository;
        private readonly IMapper _mapper;

        public FootballTeamsController(IMapper mapper, IFootballRepository repository, IFootballCosmosRepository _cosmosRepository)
        {
            _mapper = mapper;
            _repository = repository;
            this._cosmosRepository = _cosmosRepository;
        ***REMOVED***
        [HttpGet("getnewmatches")]
        public async Task<ActionResult<IEnumerable<Match>>> GetNewMatches()
        {
            List<FootballMatch> fmatches = new();
            var newMatches = await _repository.GetNewMatches();
            if (newMatches.Count() == 0)
            {
                return Ok("No matches to update");
            ***REMOVED***
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
                ***REMOVED***
            ***REMOVED***
            
            // Update teams
            foreach (var m in fmatches)
            {
                var homeTeam = await _repository.GetTeamByName(m.HomeTeam!);
                var awayTeam = await _repository.GetTeamByName(m.AwayTeam!);
                if (homeTeam != null)
                {
                    FootballTeam? hft = _repository.UpdateHomeTeam(m, homeTeam);
                    var responseUpdateht = await _repository.UpdateFootballTeam(hft!.id!, hft);
                    if (responseUpdateht == null)
                    {
                        return BadRequest("Problems while updating home team!");
                    ***REMOVED***
                ***REMOVED***
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
                    ***REMOVED***;
                    hft.Points = _repository.CalculatePoints(hft);
                    hft.GoalDifference = hft.GoalsScored - hft.GoalsLost;
                    var responseAddTeam = await _repository.AddTeam(hft);
                    if (responseAddTeam == null)
                    {
                        return BadRequest("Problems while adding home team!");
                    ***REMOVED***
                ***REMOVED***

                if (awayTeam != null)
                {
                    FootballTeam? aft = _repository.UpdateAwayTeam(m, awayTeam);
                    var responseUpdateAt = await _repository.UpdateFootballTeam(aft!.id!, aft);
                    if (responseUpdateAt == null)
                    {
                        return BadRequest("Problems while updating Away Team!");
                    ***REMOVED***
                ***REMOVED***
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
                    ***REMOVED***;
                    aft.Points = _repository.CalculatePoints(aft);
                    aft.GoalDifference = aft.GoalsScored - aft.GoalsLost;
                    var responseAddTeam = await _repository.AddTeam(aft);
                    if (responseAddTeam == null)
                    {
                        return BadRequest("Problems while adding away team!");
                    ***REMOVED***
                ***REMOVED***
            ***REMOVED***
                
            // Remove from queue
            var response = await _repository.DeleteFromQueue(newMatches);
            if (response.Count() == newMatches.Count())
            {
                return Ok(response);
            ***REMOVED***
            return BadRequest("Something went wrong while deleting!" + response);
        ***REMOVED***
        
        [Obsolete("One time job & has been run already")]
        [HttpPost("populatematchestocome")]
        public async Task PopulateMatchesToCome()
        {
            await _repository.PopulateMatchesToCome();
        ***REMOVED***

        [HttpGet("predict/{team1***REMOVED***/{team2***REMOVED***")]
        public async Task<ActionResult<string>> PredictResult(string team1, string team2)
        {
            // Get teams and check if exist
            var HomeTeam = _cosmosRepository.GetFootballTeamByName(team1);
            var AwayTeam = _cosmosRepository.GetFootballTeamByName(team2);
            if (HomeTeam == null || AwayTeam == null)
            {
                return NotFound("Team(s) not found!");
            ***REMOVED***

            
            return  await _cosmosRepository.PredictResult(team1, team2);
        ***REMOVED***
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FootballTeamDTO>>> GetTeams()
        {
            return Ok(await _cosmosRepository.GetFootballTeams());
        ***REMOVED***

        [HttpGet("{id***REMOVED***")]
        public ActionResult<FootballTeamDTO> GetFootballTeam(string id)
        {
            var team = _cosmosRepository.GetFootballTeamById(id);
            if (team == null)
            {
                return NotFound("No team with that id is in the list.");
            ***REMOVED***
            return Ok(team!);
        ***REMOVED***

        [HttpGet("getbyname/{name***REMOVED***")]
        public ActionResult<FootballTeamDTO> GetFootballTeamByName(string name)
        {
            var team = _cosmosRepository.GetFootballTeamByName(name);
            if (team == null)
            {
                return NotFound("No team with that id is in the list.");
            ***REMOVED***
            return Ok( team!);
        ***REMOVED***

        [HttpPut("{id***REMOVED***")]
        public async Task<ActionResult<FootballTeamDTO>> PutFootballTeam(string id, FootballTeamDTO footballTeam)
        {
            var teamToUpdate = _cosmosRepository.GetFootballTeamById(id);
            if (teamToUpdate == null)
            {
                return NotFound("No team with that id found");
            ***REMOVED***

            FootballTeam teamToChange = _mapper.Map<FootballTeam>(footballTeam);
            teamToChange.id = id;
            return Ok(_cosmosRepository.UpdateFootballTeam(id, teamToChange));
        ***REMOVED***


        [HttpPost]
        public async Task<ActionResult<FootballTeam>> PostFootballTeam(FootballTeamDTO footballTeam)
        {
            if (_cosmosRepository.GetFootballTeamByName(footballTeam.Name!) != null)
            {
                return Problem("A team with that name is already in the list!");
            ***REMOVED***
            var result = _cosmosRepository.AddFootballTeam(footballTeam);
            return result ? Ok(_mapper.Map<FootballTeamDTO>(footballTeam)) : BadRequest("Error when trying to add team to list");
        ***REMOVED***


        [HttpDelete("{id***REMOVED***")]
        public  ActionResult<FootballTeam> DeleteFootballTeam(string id)
        {
            if (_cosmosRepository.GetFootballTeamById(id) == null)
            {
                return NotFound("No team with that id in the list");
            ***REMOVED***

            return Ok(_cosmosRepository.DeleteFootballTeamById(id));
        ***REMOVED***

        [HttpDelete("deletebyname/{name***REMOVED***")]
        public ActionResult<FootballTeam> DeleteFootballTeamByName(string name)
        {
            if (_cosmosRepository.GetFootballTeamByName(name) == null)
            {
                return NotFound("No team with that name in the list");
            ***REMOVED***

            return Ok(_cosmosRepository.DeleteFootballTeamByName(name));
        ***REMOVED***

        [Obsolete("Not needed after population is done")]
        [HttpPost("populateteams")]
        public void PopulateTeams()
        {
            _repository.PopulateTeams();
        ***REMOVED***
        
        [Obsolete("Not needed after initial population of db")]

        [HttpPost("populatematches")]
        public async Task PopulateMatches()
        {
            await _repository.PopulateMatches();
        ***REMOVED***

        [HttpGet("queue/getMatches")]
        public IEnumerable<Match> GetMatchesQueue()
        {
            return _cosmosRepository.GetMatchesQueue();
        ***REMOVED***
    ***REMOVED***
***REMOVED***

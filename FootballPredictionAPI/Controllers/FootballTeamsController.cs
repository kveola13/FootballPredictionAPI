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
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos;

namespace FootballPredictionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FootballTeamsController : ControllerBase
    {
        private readonly FootballTeamContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly string containerName = "teams";

        public FootballTeamsController(FootballTeamContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("seed")]
        public void SeedFootballTeam()
        {
            if (_context.Teams.FirstOrDefault(fb => fb.Name!.ToLower().Equals("fc barcelona")) == null)
            {
                var fb = new FootballTeamDTO
                {
                    Name = "FC Barcelona",
                    MatchesWon = 3,
                    MatchesLost = 2,
                    MatchesDraw = 1,
                    Points = 0,
                    Description = "Team from Barcelona"
                };
                fb.Points = CalculatePoints(_mapper.Map<FootballTeam>(fb));
                _context.Teams.Add(_mapper.Map<FootballTeam>(fb));
                _context.SaveChangesAsync();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FootballTeamDTO>>> GetTeams()
        {
            CreateDatabaseConnection(out _, out Container container);
            using FeedIterator<FootballTeam> feed = container.GetItemQueryIterator<FootballTeam>(
                queryText: "SELECT * FROM c"
            );
            if (!feed.HasMoreResults)
            {
                return NotFound("No teams found in database");
            }
            List<FootballTeamDTO> teams = new();
            while (feed.HasMoreResults)
            {
                FeedResponse<FootballTeam> response = await feed.ReadNextAsync();
                foreach (FootballTeam teamDTO in response)
                {
                    Console.WriteLine($"Team found: {teamDTO.Id}");
                    teams.Add(_mapper.Map<FootballTeamDTO>(teamDTO));
                }
            }
            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FootballTeamDTO>> GetFootballTeam(string id)
        {
            CreateDatabaseConnection(out _, out Container container);
            IOrderedQueryable<FootballTeam> queryable = container.GetItemLinqQueryable<FootballTeam>();
            var matches = queryable
            .Where(fb => fb!.Id!.Equals(id));
            using FeedIterator<FootballTeam> linqFeed = matches.ToFeedIterator();
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<FootballTeam> response = await linqFeed.ReadNextAsync();
                foreach (FootballTeam team in response)
                {
                    Console.WriteLine($"Found team: {team.Name} with id: {team.Id}");
                    var teamDto = _mapper.Map<FootballTeamDTO>(team);
                    return Ok(teamDto);
                }
            }
            return NotFound("No such team found in database");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFootballTeam(string id, FootballTeamDTO footballTeam)
        {
            FootballTeam teamToChange = _mapper.Map<FootballTeam>(footballTeam);
            teamToChange.Id = id;
            _context.Entry(teamToChange).State = EntityState.Detached;
            _context.Teams.Update(teamToChange);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FootballTeamExists(id))
                {
                    return NotFound("No team with that ID found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<FootballTeamDTO>> PostFootballTeam(FootballTeam footballTeam)
        {
            CreateDatabaseConnection(out _, out Container container);
            
            footballTeam.Points = CalculatePoints(footballTeam);
            await container.CreateItemAsync(footballTeam!);

            return CreatedAtAction(nameof(GetFootballTeam), new { id = "testing" },_mapper.Map<FootballTeamDTO>(footballTeam));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFootballTeam(int id)
        {

            var footballTeam = await _context.Teams.FindAsync(id);
            if (footballTeam == null)
            {
                return NotFound();
            }

            _context.Teams.Remove(footballTeam);
            await _context.SaveChangesAsync();

            return Ok($"{footballTeam.Name} has been removed");
        }

        private static int CalculatePoints(FootballTeam footballTeam)
        {
            return (footballTeam.MatchesWon * 3) + footballTeam.MatchesDraw;
        }
        private bool FootballTeamExists(string id)
        {
            return (_context.Teams?.Any(e => e.Id!.Equals(id))).GetValueOrDefault();
        }
        private void CreateDatabaseConnection(out CosmosClient client, out Container container)
        {
            var dbName = _configuration["ConnectionStrings:DATABASE_NAME"]!;
            var accountEndpoint = _configuration["ConnectionStrings:COSMOS_ENDPOINT"]!;
            var accountKey = _configuration["ConnectionStrings:COSMOS_KEY"]!;
            client = new
            (
                accountEndpoint: accountEndpoint,
                authKeyOrResourceToken: accountKey!
            );
            container = client.GetContainer(dbName, containerName);
        }

    }
}

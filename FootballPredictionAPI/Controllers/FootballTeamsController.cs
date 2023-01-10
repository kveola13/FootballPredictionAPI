using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballPredictionAPI.Models;

namespace FootballPredictionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FootballTeamsController : ControllerBase
    {
        private readonly FootballTeamContext _context;

        public FootballTeamsController(FootballTeamContext context)
        {
            _context = context;
        }

        [HttpPost("seed")]
        public void SeedFootballTeam()
        {
            if (_context.Teams.FirstOrDefault(fb=> fb.Name.ToLower().Equals("fc barcelona")) == null)
            {
                var fb = new FootballTeam
                {
                    Name = "FC Barcelona",
                    Points = 10,
                    MatchesWon = 3,
                    MatchesLost = 2,
                    MatchesDraw = 1,
                    Description = "Team from Barcelona"
                };
                _context.Teams.Add(fb);
                _context.SaveChangesAsync();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FootballTeam>>> GetTeams()
        {
          if (_context.Teams == null)
          {
              return NotFound();
          }
            return await _context.Teams.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FootballTeam>> GetFootballTeam(int id)
        {
          if (_context.Teams == null)
          {
              return NotFound();
          }
            var footballTeam = await _context.Teams.FindAsync(id);

            if (footballTeam == null)
            {
                return NotFound();
            }

            return footballTeam;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFootballTeam(int id, FootballTeam footballTeam)
        {
            if (id != footballTeam.Id)
            {
                return BadRequest();
            }

            _context.Entry(footballTeam).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FootballTeamExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<FootballTeam>> PostFootballTeam(FootballTeam footballTeam)
        {
          if (_context.Teams == null)
          {
              return Problem("Entity set 'FootballTeamContext.Teams'  is null.");
          }
            _context.Teams.Add(footballTeam);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFootballTeam), new { id = footballTeam.Id }, footballTeam);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFootballTeam(int id)
        {
            if (_context.Teams == null)
            {
                return NotFound();
            }
            var footballTeam = await _context.Teams.FindAsync(id);
            if (footballTeam == null)
            {
                return NotFound();
            }

            _context.Teams.Remove(footballTeam);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FootballTeamExists(int id)
        {
            return (_context.Teams?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

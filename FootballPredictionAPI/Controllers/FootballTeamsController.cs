using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballPredictionAPI.Models;
using System.Reflection.Emit;

namespace FootballPredictionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FootballTeamsController : ControllerBase
    {
        private readonly FootballDB _context;

        public FootballTeamsController(FootballDB context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FootballTeam>>> GetFootballDBs()
        {
          if (_context.FootballDBs == null)
          {
              return NotFound();
          }
            return await _context.FootballDBs.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FootballTeam>> GetFootballTeam(int id)
        {
          if (_context.FootballDBs == null)
          {
              return NotFound();
          }
            var footballTeam = await _context.FootballDBs.FindAsync(id);

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
          if (_context.FootballDBs == null)
          {
              return Problem("Entity set 'FootballDB.FootballDBs'  is null.");
          }
            _context.FootballDBs.Add(footballTeam);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(FootballTeam), new { id = footballTeam.Id }, footballTeam);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFootballTeam(int id)
        {
            if (_context.FootballDBs == null)
            {
                return NotFound();
            }
            var footballTeam = await _context.FootballDBs.FindAsync(id);
            if (footballTeam == null)
            {
                return NotFound();
            }

            _context.FootballDBs.Remove(footballTeam);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FootballTeamExists(int id)
        {
            return (_context.FootballDBs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

﻿using System;
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

namespace FootballPredictionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FootballTeamsController : ControllerBase
    {
        private readonly FootballTeamContext _context;
        private readonly IMapper _mapper;

        public FootballTeamsController(FootballTeamContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("seed")]
        public void SeedFootballTeam()
        {
            if (_context.Teams.FirstOrDefault(fb=> fb.Name.ToLower().Equals("fc barcelona")) == null)
            {
                var fb = new FootballTeam
                {
                    Name = "FC Barcelona",
                    MatchesWon = 3,
                    MatchesLost = 2,
                    MatchesDraw = 1,
                    Points = 0,
                    Description = "Team from Barcelona"
                };
                fb.Points = CalculatePoints(fb);
                _context.Teams.Add(fb);
                _context.SaveChangesAsync();
            }
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FootballTeamDTO>>> GetTeams()
        {
          if (_context.Teams == null)
          {
              return NotFound();
          }
             return Ok(_mapper.Map<IEnumerable<FootballTeamDTO>>(await _context.Teams.ToListAsync()));
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<FootballTeamDTO>> GetFootballTeam(int id)
        {
          if (_context.Teams == null)
          {
              return NotFound();
          }
            var footballTeam = _mapper.Map<FootballTeamDTO>(await _context.Teams.FindAsync(id));

            if (footballTeam == null)
            {
                return NotFound();
            }

            return footballTeam;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFootballTeam(int id, FootballTeamDTO footballTeam)
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
        public async Task<ActionResult<FootballTeam>> PostFootballTeam(FootballTeam footballTeam)
        {
          if (_context.Teams == null)
          {
              return Problem("Entity set 'FootballTeamContext.Teams'  is null.");
          }

          if (_context.Teams.FirstOrDefault(team => team.Name.ToLower().Equals(footballTeam.Name.ToLower())) != null)
          {
              return Problem("A team with that name is already in the list!");
          }
          footballTeam.Points = CalculatePoints(footballTeam);
            _context.Teams.Add(footballTeam);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFootballTeam), new { id = footballTeam.Id }, footballTeam);
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
        
        private int CalculatePoints(FootballTeam footballTeam)
        {
            return (footballTeam.MatchesWon * 3) + footballTeam.MatchesDraw;
        }
        private bool FootballTeamExists(int id)
        {
            return (_context.Teams?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
    }
}

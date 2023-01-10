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
        private readonly FootballDB _context;

        public FootballTeamsController(FootballDB context)
        {
            _context = context;
        ***REMOVED***

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FootballTeam>>> GetFootballDBs()
        {
          if (_context.FootballDBs == null)
          {
              return NotFound();
          ***REMOVED***
            return await _context.FootballDBs.ToListAsync();
        ***REMOVED***

        [HttpGet("{id***REMOVED***")]
        public async Task<ActionResult<FootballTeam>> GetFootballTeam(int id)
        {
          if (_context.FootballDBs == null)
          {
              return NotFound();
          ***REMOVED***
            var footballTeam = await _context.FootballDBs.FindAsync(id);

            if (footballTeam == null)
            {
                return NotFound();
            ***REMOVED***

            return footballTeam;
        ***REMOVED***

        [HttpPut("{id***REMOVED***")]
        public async Task<IActionResult> PutFootballTeam(int id, FootballTeam footballTeam)
        {
            if (id != footballTeam.Id)
            {
                return BadRequest();
            ***REMOVED***

            _context.Entry(footballTeam).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            ***REMOVED***
            catch (DbUpdateConcurrencyException)
            {
                if (!FootballTeamExists(id))
                {
                    return NotFound();
                ***REMOVED***
                else
                {
                    throw;
                ***REMOVED***
            ***REMOVED***

            return NoContent();
        ***REMOVED***

        [HttpPost]
        public async Task<ActionResult<FootballTeam>> PostFootballTeam(FootballTeam footballTeam)
        {
          if (_context.FootballDBs == null)
          {
              return Problem("Entity set 'FootballDB.FootballDBs'  is null.");
          ***REMOVED***
            _context.FootballDBs.Add(footballTeam);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFootballTeam", new { id = footballTeam.Id ***REMOVED***, footballTeam);
        ***REMOVED***

        [HttpDelete("{id***REMOVED***")]
        public async Task<IActionResult> DeleteFootballTeam(int id)
        {
            if (_context.FootballDBs == null)
            {
                return NotFound();
            ***REMOVED***
            var footballTeam = await _context.FootballDBs.FindAsync(id);
            if (footballTeam == null)
            {
                return NotFound();
            ***REMOVED***

            _context.FootballDBs.Remove(footballTeam);
            await _context.SaveChangesAsync();

            return NoContent();
        ***REMOVED***

        private bool FootballTeamExists(int id)
        {
            return (_context.FootballDBs?.Any(e => e.Id == id)).GetValueOrDefault();
        ***REMOVED***
    ***REMOVED***
***REMOVED***

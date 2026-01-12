using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Server.DB;
using CodeChallenge.Server.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using CodeChallenge.Server.Helpers;
using System.Globalization;

namespace CodeChallenge.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamStatsController : ControllerBase
    {
        private readonly CollegeFootballContext _context;

        public TeamStatsController(CollegeFootballContext context)
        {
            _context = context;
        }

        // GET: api/TeamStats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamStats>>> GetTeamStats([FromQuery] int selectedOption, [FromQuery] string searchText)
        {
            var query = _context.TeamStats.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {

                searchText = searchText.ToLower();

                var hasIntSearch = int.TryParse(searchText, out var intSearch);
                var hasDoubleSearch = double.TryParse(searchText, out var doubleSearch);
                var hasDateSearch = DateOnly.TryParseExact(searchText, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateSearch);
                

                query = selectedOption switch
                {
                    1 => hasIntSearch  ? query.Where(x => x.Rank == intSearch)  : query.Where(x => false),
                    2 => query.Where(x => x.Team.ToLower().Contains(searchText)),
                    3 => query.Where(x => x.Mascot.ToLower().Contains(searchText)),
                    4 => query.Where(x => x.LastWinDate == searchText),
                    5 => hasDoubleSearch ? query.Where(x => x.Percentage == doubleSearch): query.Where(x => false),
                    6 => hasIntSearch ? query.Where(x => x.Wins == intSearch) : query.Where(x => false),
                    7 => hasIntSearch ? query.Where(x => x.Losses == intSearch) : query.Where(x => false),
                    8 => hasIntSearch ? query.Where(x => x.Ties == intSearch) : query.Where(x => false),
                    9 => hasIntSearch ? query.Where(x => x.Games == intSearch) : query.Where(x => false),
                    0 or _ => query.Where(x =>
                                    (hasIntSearch && x.Rank == intSearch) ||
                                    x.Team.ToLower().Contains(searchText) ||
                                    x.Mascot.ToLower().Contains(searchText) ||
                                    (hasDateSearch && x.LastWinDate == searchText) ||
                                    (hasDoubleSearch && x.Percentage == doubleSearch) ||
                                    (hasIntSearch && x.Wins == intSearch) ||
                                    (hasIntSearch && x.Losses == intSearch) ||
                                    (hasIntSearch && x.Ties == intSearch) ||
                                    (hasIntSearch && x.Games == intSearch))
                };
            }

            return await query.ToListAsync();
        }

        // GET: api/TeamStats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamStats>> GetTeamStats(int id)
        {
            var teamStats = await _context.TeamStats.FindAsync(id);

            if (teamStats == null)
            {
                return NotFound();
            }

            return teamStats;
        }

        // PUT: api/TeamStats/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeamStats(int id, TeamStats teamStats)
        {
            if (id != teamStats.TeamStatsID)
            {
                return BadRequest();
            }

            _context.Entry(teamStats).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamStatsExists(id))
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

        // POST: api/TeamStats
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TeamStats>> PostTeamStats(TeamStats teamStats)
        {
            _context.TeamStats.Add(teamStats);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeamStats", new { id = teamStats.TeamStatsID }, teamStats);
        }

        // DELETE: api/TeamStats/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamStats(int id)
        {
            var teamStats = await _context.TeamStats.FindAsync(id);
            if (teamStats == null)
            {
                return NotFound();
            }

            _context.TeamStats.Remove(teamStats);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeamStatsExists(int id)
        {
            return _context.TeamStats.Any(e => e.TeamStatsID == id);
        }
    }
}

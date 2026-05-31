using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using libraryinfrastructure;
using tournamentdomain.Model;

namespace tournament_infrastructure.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly DbTournamentContext _context;

        public StatisticsController(DbTournamentContext context)
        {
            _context = context;
        }

        // GET: Statistics
        public async Task<IActionResult> Index()
        {
            var dbTournamentContext = _context.Statistics.Include(s => s.Match).Include(s => s.TeamNameNavigation);
            return View(await dbTournamentContext.ToListAsync());
        }

        // GET: Statistics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistics
                .Include(s => s.Match)
                .Include(s => s.TeamNameNavigation)
                .FirstOrDefaultAsync(m => m.MatchId == id);
            if (statistic == null)
            {
                return NotFound();
            }

            return View(statistic);
        }

        // GET: Statistics/Create
        public IActionResult Create()
        {
            ViewData["MatchId"] = new SelectList(_context.Matches, "Id", "Id");
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name");
            return View();
        }

        // POST: Statistics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MatchId,TeamName,Kills,Deaths,Damage")] Statistic statistic)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statistic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MatchId"] = new SelectList(_context.Matches, "Id", "Id", statistic.MatchId);
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name", statistic.TeamName);
            return View(statistic);
        }

        // GET: Statistics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistics.FindAsync(id);
            if (statistic == null)
            {
                return NotFound();
            }
            ViewData["MatchId"] = new SelectList(_context.Matches, "Id", "Id", statistic.MatchId);
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name", statistic.TeamName);
            return View(statistic);
        }

        // POST: Statistics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MatchId,TeamName,Kills,Deaths,Damage")] Statistic statistic)
        {
            if (id != statistic.MatchId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statistic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatisticExists(statistic.MatchId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MatchId"] = new SelectList(_context.Matches, "Id", "Id", statistic.MatchId);
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name", statistic.TeamName);
            return View(statistic);
        }

        // GET: Statistics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistics
                .Include(s => s.Match)
                .Include(s => s.TeamNameNavigation)
                .FirstOrDefaultAsync(m => m.MatchId == id);
            if (statistic == null)
            {
                return NotFound();
            }

            return View(statistic);
        }

        // POST: Statistics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var statistic = await _context.Statistics.FindAsync(id);
            if (statistic != null)
            {
                _context.Statistics.Remove(statistic);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatisticExists(int id)
        {
            return _context.Statistics.Any(e => e.MatchId == id);
        }
    }
}

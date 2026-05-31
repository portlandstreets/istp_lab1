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
    public class TeamsController : Controller
    {
        private readonly DbTournamentContext _context;

        public TeamsController(DbTournamentContext context)
        {
            _context = context;
        }

        // GET: Teams/Stats
        public IActionResult Stats()
        {
            return View();
        }

        // GET: Teams/StatsData
        public async Task<JsonResult> StatsData()
        {
            var teams = await _context.Teams
                .Select(t => new { Name = t.Name, Points = t.RankingPoints })
                .ToListAsync();

            // Build array of arrays for Google Charts: [ ['Team','Points'], ['A', 10], ... ]
            var rows = new List<object[]>();
            rows.Add(new object[] { "Team", "Points" });
            foreach (var t in teams)
            {
                rows.Add(new object[] { t.Name ?? string.Empty, t.Points });
            }

            return Json(rows);
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var dbTournamentContext = _context.Teams.Include(t => t.Coach);
            return View(await dbTournamentContext.ToListAsync());
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.Coach)
                .FirstOrDefaultAsync(m => m.Name == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            ViewData["CoachId"] = new SelectList(_context.Coaches, "Id", "Id");
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Country,CreationYear,RankingPoints,CoachId")] Team team)
        {
            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CoachId"] = new SelectList(_context.Coaches, "Id", "Id", team.CoachId);
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            ViewData["CoachId"] = new SelectList(_context.Coaches, "Id", "Id", team.CoachId);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Country,CreationYear,RankingPoints,CoachId")] Team team)
        {
            if (id != team.Name)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Name))
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
            ViewData["CoachId"] = new SelectList(_context.Coaches, "Id", "Id", team.CoachId);
            return View(team);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.Coach)
                .FirstOrDefaultAsync(m => m.Name == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                _context.Teams.Remove(team);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(string id)
        {
            return _context.Teams.Any(e => e.Name == id);
        }
    }
}

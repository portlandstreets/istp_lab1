using libraryinfrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tournamentdomain.Model;

namespace tournament_infrastructure.Controllers
{
    public class TournamentsController : Controller
    {
        private readonly DbTournamentContext _context;

        public TournamentsController(DbTournamentContext context)
        {
            _context = context;
        }

        // GET: Tournaments/Export
        public async Task<IActionResult> Export()
        {
            try
            {
                using var ms = new System.IO.MemoryStream();
                var exporter = new libraryinfrastructure.Services.TournamentExportService(_context);
                await exporter.WriteToAsync(ms, System.Threading.CancellationToken.None);
                ms.Position = 0;
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"tournaments_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
                return File(ms.ToArray(), contentType, fileName);
            }
            catch (Exception ex)
            {
                // You may want to log the exception
                TempData["Error"] = "Помилка при створенні файлу експорту: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Tournaments
        public async Task<IActionResult> Index()
        {
            var dbTournamentContext = _context.Tournaments.Include(t => t.OrganizerNameNavigation);
            return View(await dbTournamentContext.ToListAsync());
        }

        // GET: Tournaments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .Include(t => t.OrganizerNameNavigation)
                .FirstOrDefaultAsync(m => m.Name == id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        // GET: Tournaments/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["OrganizerName"] = new SelectList(_context.Organizers, "Name", "Name");
            return View();
        }

        // POST: Tournaments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Location,StartDate,EndDate,PrizePool,OrganizerName")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizerName"] = new SelectList(_context.Organizers, "Name", "Name", tournament.OrganizerName);
            return View(tournament);
        }

        // GET: Tournaments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }
            ViewData["OrganizerName"] = new SelectList(_context.Organizers, "Name", "Name", tournament.OrganizerName);
            return View(tournament);
        }

        // POST: Tournaments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Location,StartDate,EndDate,PrizePool,OrganizerName")] Tournament tournament)
        {
            if (id != tournament.Name)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournament);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentExists(tournament.Name))
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
            ViewData["OrganizerName"] = new SelectList(_context.Organizers, "Name", "Name", tournament.OrganizerName);
            return View(tournament);
        }

        // GET: Tournaments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .Include(t => t.OrganizerNameNavigation)
                .FirstOrDefaultAsync(m => m.Name == id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        // POST: Tournaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament != null)
            {
                _context.Tournaments.Remove(tournament);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TournamentExists(string id)
        {
            return _context.Tournaments.Any(e => e.Name == id);
        }
    }
}

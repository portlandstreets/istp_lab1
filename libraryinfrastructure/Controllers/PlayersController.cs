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
    public class PlayersController : Controller
    {
        private readonly DbTournamentContext _context;

        public PlayersController(DbTournamentContext context)
        {
            _context = context;
        }

        // GET: Players
        public async Task<IActionResult> Index()
        {
            var dbTournamentContext = _context.Players.Include(p => p.SubstituteForNavigation).Include(p => p.TeamNameNavigation);
            return View(await dbTournamentContext.ToListAsync());
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.SubstituteForNavigation)
                .Include(p => p.TeamNameNavigation)
                .FirstOrDefaultAsync(m => m.Nickname == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Players/Create
        public IActionResult Create()
        {
            ViewData["SubstituteFor"] = new SelectList(_context.Players, "Nickname", "Nickname");
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name");
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nickname,RealName,BirthDate,Country,Role,TeamName,SubstituteFor")] Player player)
        {
            if (ModelState.IsValid)
            {
                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubstituteFor"] = new SelectList(_context.Players, "Nickname", "Nickname", player.SubstituteFor);
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name", player.TeamName);
            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            ViewData["SubstituteFor"] = new SelectList(_context.Players, "Nickname", "Nickname", player.SubstituteFor);
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name", player.TeamName);
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nickname,RealName,BirthDate,Country,Role,TeamName,SubstituteFor")] Player player)
        {
            if (id != player.Nickname)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.Nickname))
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
            ViewData["SubstituteFor"] = new SelectList(_context.Players, "Nickname", "Nickname", player.SubstituteFor);
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name", player.TeamName);
            return View(player);
        }

        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.SubstituteForNavigation)
                .Include(p => p.TeamNameNavigation)
                .FirstOrDefaultAsync(m => m.Nickname == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlayerExists(string id)
        {
            return _context.Players.Any(e => e.Nickname == id);
        }
    }
}

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
    public class CaptainsController : Controller
    {
        private readonly DbTournamentContext _context;

        public CaptainsController(DbTournamentContext context)
        {
            _context = context;
        }

        // GET: Captains
        public async Task<IActionResult> Index()
        {
            var dbTournamentContext = _context.Captains.Include(c => c.NicknameNavigation).Include(c => c.TeamNameNavigation);
            return View(await dbTournamentContext.ToListAsync());
        }

        // GET: Captains/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var captain = await _context.Captains
                .Include(c => c.NicknameNavigation)
                .Include(c => c.TeamNameNavigation)
                .FirstOrDefaultAsync(m => m.Nickname == id);
            if (captain == null)
            {
                return NotFound();
            }

            return View(captain);
        }

        // GET: Captains/Create
        public IActionResult Create()
        {
            ViewData["Nickname"] = new SelectList(_context.Players, "Nickname", "Nickname");
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name");
            return View();
        }

        // POST: Captains/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nickname,TeamName")] Captain captain)
        {
            if (ModelState.IsValid)
            {
                _context.Add(captain);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Nickname"] = new SelectList(_context.Players, "Nickname", "Nickname", captain.Nickname);
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name", captain.TeamName);
            return View(captain);
        }

        // GET: Captains/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var captain = await _context.Captains.FindAsync(id);
            if (captain == null)
            {
                return NotFound();
            }
            ViewData["Nickname"] = new SelectList(_context.Players, "Nickname", "Nickname", captain.Nickname);
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name", captain.TeamName);
            return View(captain);
        }

        // POST: Captains/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nickname,TeamName")] Captain captain)
        {
            if (id != captain.Nickname)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(captain);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CaptainExists(captain.Nickname))
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
            ViewData["Nickname"] = new SelectList(_context.Players, "Nickname", "Nickname", captain.Nickname);
            ViewData["TeamName"] = new SelectList(_context.Teams, "Name", "Name", captain.TeamName);
            return View(captain);
        }

        // GET: Captains/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var captain = await _context.Captains
                .Include(c => c.NicknameNavigation)
                .Include(c => c.TeamNameNavigation)
                .FirstOrDefaultAsync(m => m.Nickname == id);
            if (captain == null)
            {
                return NotFound();
            }

            return View(captain);
        }

        // POST: Captains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var captain = await _context.Captains.FindAsync(id);
            if (captain != null)
            {
                _context.Captains.Remove(captain);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CaptainExists(string id)
        {
            return _context.Captains.Any(e => e.Nickname == id);
        }
    }
}

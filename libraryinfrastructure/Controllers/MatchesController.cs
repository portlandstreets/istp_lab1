using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel; // Додано для роботи з Excel
using libraryinfrastructure;
using tournamentdomain.Model;
// using libraryinfrastructure.Services; // Закоментовано, бо тепер парсимо файл напряму

namespace tournament_infrastructure.Controllers
{
    public class MatchesController : Controller
    {
        private readonly DbTournamentContext _context;
        private readonly libraryinfrastructure.Services.IDataPortServiceFactory<Match> _factory;

        public MatchesController(DbTournamentContext context, libraryinfrastructure.Services.IDataPortServiceFactory<Match> factory)
        {
            _context = context;
            _factory = factory;
        }

        // GET: Matches/ExportDoc
        public async Task<IActionResult> ExportDoc()
        {
            try
            {
                var matches = await _context.Matches
                    .Include(m => m.TournamentNameNavigation)
                    .Include(m => m.WinnerTeamNavigation)
                    .AsNoTracking()
                    .ToListAsync();

                var sb = new System.Text.StringBuilder();
                // Simple RTF header
                sb.AppendLine("{\\rtf1\\ansi\\deff0");
                sb.AppendLine("{\\fonttbl{\\f0 Calibri;}}");
                sb.AppendLine("\\f0\\fs24");
                sb.AppendLine("Matches:\\par");

                foreach (var m in matches)
                {
                    var date = m.MatchDate?.ToString("yyyy-MM-dd HH:mm") ?? "";
                    var stage = EscapeRtf(m.Stage);
                    var duration = m.Duration?.ToString() ?? "";
                    var tournament = EscapeRtf(m.TournamentNameNavigation?.Name ?? m.TournamentName ?? "");
                    var winner = EscapeRtf(m.WinnerTeamNavigation?.Name ?? m.WinnerTeam ?? "");

                    sb.AppendLine($"Id: {m.Id}\\tab Date: {date}\\tab Stage: {stage}\\tab Duration: {duration}\\tab Tournament: {tournament}\\tab Winner: {winner}\\par");
                }

                sb.AppendLine("}");

                var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                var fileName = $"matches_{DateTime.UtcNow:yyyyMMddHHmmss}.doc";
                return File(bytes, "application/msword", fileName);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Помилка при створенні документа: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        private static string EscapeRtf(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace("\\", "\\\\").Replace("{", "\\{").Replace("}", "\\}");
        }

        // GET: Matches/Import
        public IActionResult Import()
        {
            return View();
        }

        // GET: Matches/Export
        public async Task<IActionResult> Export()
        {
            try
            {
                using var ms = new MemoryStream();
                var exporter = new libraryinfrastructure.Services.MatchExportService(_context);
                await exporter.WriteToAsync(ms, System.Threading.CancellationToken.None);
                ms.Position = 0;
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"matches_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
                return File(ms.ToArray(), contentType, fileName);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Помилка при створенні файлу експорту: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Matches/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (fileExcel == null || fileExcel.Length == 0)
            {
                ModelState.AddModelError("fileExcel", "Будь ласка, виберіть файл.");
                return View();
            }

            try
            {
                using var stream = fileExcel.OpenReadStream();
                var contentType = fileExcel.ContentType ?? string.Empty;

                var importer = _factory.GetImportService(contentType);
                await importer.ImportFromStreamAsync(stream, HttpContext.RequestAborted);
            }
            catch (NotSupportedException ns)
            {
                ModelState.AddModelError(string.Empty, ns.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Помилка під час читання/збереження файлу: " + ex.Message);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Matches
        // Додано параметр 'q' для підтримки пошуку з форми
        public async Task<IActionResult> Index(string q)
        {
            // Базовий запит із підключенням навігаційних властивостей 
            var matchesQuery = _context.Matches
                .Include(m => m.TournamentNameNavigation)
                .Include(m => m.WinnerTeamNavigation)
                .AsQueryable();

            // Логіка фільтрації, якщо введено пошуковий запит
            if (!string.IsNullOrEmpty(q))
            {
                matchesQuery = matchesQuery.Where(m =>
                    m.Stage.Contains(q) ||
                    m.TournamentNameNavigation.Name.Contains(q) ||
                    m.WinnerTeamNavigation.Name.Contains(q));
            }

            return View(await matchesQuery.ToListAsync());
        }

        // GET: Matches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Matches
                .Include(m => m.TournamentNameNavigation)
                .Include(m => m.WinnerTeamNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        // GET: Matches/Create
        public IActionResult Create()
        {
            ViewData["TournamentName"] = new SelectList(_context.Tournaments, "Name", "Name");
            ViewData["WinnerTeam"] = new SelectList(_context.Teams, "Name", "Name");
            return View();
        }

        // POST: Matches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MatchDate,Stage,Duration,WinnerTeam,TournamentName")] Match match)
        {
            if (ModelState.IsValid)
            {
                _context.Add(match);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TournamentName"] = new SelectList(_context.Tournaments, "Name", "Name", match.TournamentName);
            ViewData["WinnerTeam"] = new SelectList(_context.Teams, "Name", "Name", match.WinnerTeam);
            return View(match);
        }

        // GET: Matches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }
            ViewData["TournamentName"] = new SelectList(_context.Tournaments, "Name", "Name", match.TournamentName);
            ViewData["WinnerTeam"] = new SelectList(_context.Teams, "Name", "Name", match.WinnerTeam);
            return View(match);
        }

        // POST: Matches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MatchDate,Stage,Duration,WinnerTeam,TournamentName")] Match match)
        {
            if (id != match.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(match);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchExists(match.Id))
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
            ViewData["TournamentName"] = new SelectList(_context.Tournaments, "Name", "Name", match.TournamentName);
            ViewData["WinnerTeam"] = new SelectList(_context.Teams, "Name", "Name", match.WinnerTeam);
            return View(match);
        }

        // GET: Matches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Matches
                .Include(m => m.TournamentNameNavigation)
                .Include(m => m.WinnerTeamNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match != null)
            {
                _context.Matches.Remove(match);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatchExists(int id)
        {
            return _context.Matches.Any(e => e.Id == id);
        }
    }
}
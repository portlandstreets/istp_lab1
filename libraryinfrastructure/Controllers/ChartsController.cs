using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using libraryinfrastructure;
using tournamentdomain.Model;

namespace tournament_infrastructure.Controllers
{
    public class ChartsController : Controller
    {
        private readonly DbTournamentContext _context;

        public ChartsController(DbTournamentContext context)
        {
            _context = context;
        }

        // GET: Charts/PrizePools
        public IActionResult PrizePools()
        {
            return View();
        }

        // GET: Charts/PrizePoolsData
        public async Task<JsonResult> PrizePoolsData()
        {
            var data = await _context.Tournaments
                .Select(t => new { Name = t.Name, PrizePool = t.PrizePool })
                .ToListAsync();

            var rows = new List<object[]>();
            rows.Add(new object[] { "Tournament", "PrizePool" });

            foreach (var d in data)
            {
                rows.Add(new object[] { d.Name ?? string.Empty, d.PrizePool });
            }

            return Json(rows);
        }
    }
}


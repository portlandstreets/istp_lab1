using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using tournamentdomain.Model;

namespace libraryinfrastructure.Services
{
    public class MatchImportService : IImportService<Match>
    {
        private readonly DbTournamentContext _context;

        public MatchImportService(DbTournamentContext context)
        {
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null || !stream.CanRead)
            {
                throw new ArgumentException("Stream is not readable", nameof(stream));
            }

            using var workBook = new XLWorkbook(stream);

            foreach (IXLWorksheet worksheet in workBook.Worksheets)
            {
                var tournamentName = worksheet.Name?.Trim();
                if (string.IsNullOrEmpty(tournamentName))
                    continue;

                var tournament = await _context.Tournaments
                    .FirstOrDefaultAsync(t => t.Name == tournamentName, cancellationToken);

                if (tournament == null)
                {
                    tournament = new Tournament { Name = tournamentName };
                    _context.Tournaments.Add(tournament);
                }

                var rows = worksheet.RowsUsed().Skip(1);
                foreach (var row in rows)
                {
                    await AddMatchAsync(row, cancellationToken, tournament);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task AddMatchAsync(IXLRow row, CancellationToken cancellationToken, Tournament tournament)
        {
            var match = new Match();

            // Link by tournament name
            match.TournamentName = tournament.Name;

            // Column A (1) - MatchDate
            var dateStr = row.Cell(1).GetString();
            if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var matchDate))
            {
                match.MatchDate = matchDate;
            }

            // Column B (2) - Stage
            match.Stage = row.Cell(2).GetString();

            // Column C (3) - Duration: try parse TimeSpan or numeric minutes
            var durStr = row.Cell(3).GetString();
            if (TimeSpan.TryParse(durStr, out var ts))
            {
                match.Duration = ts;
            }
            else if (double.TryParse(durStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            {
                // interpret numeric as minutes
                match.Duration = TimeSpan.FromMinutes(d);
            }
            else
            {
                // try to get as Excel time (serial) if cell is numeric
                if (row.Cell(3).TryGetValue(out double excelVal))
                {
                    match.Duration = TimeSpan.FromDays(excelVal);
                }
            }

            // Column D (4) - WinnerTeam (set only if team exists)
            var winner = row.Cell(4).GetString();
            if (!string.IsNullOrWhiteSpace(winner))
            {
                var team = await _context.Teams.FirstOrDefaultAsync(t => t.Name == winner.Trim(), cancellationToken);
                match.WinnerTeam = team?.Name; // null if not found
            }

            _context.Matches.Add(match);
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using tournamentdomain.Model;
using libraryinfrastructure;

namespace libraryinfrastructure.Services
{
    public class MatchExportService : IExportService<Match>
    {
        private static readonly IReadOnlyList<string> HeaderNames = new[]
        {
            "Id", "MatchDate", "Stage", "Duration", "Tournament", "Winner"
        };

        private readonly DbTournamentContext _context;

        public MatchExportService(DbTournamentContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null || !stream.CanWrite)
                throw new ArgumentException("Stream is not writable", nameof(stream));

            var matches = await _context.Matches
                .Include(m => m.TournamentNameNavigation)
                .Include(m => m.WinnerTeamNavigation)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Matches");

            for (int i = 0; i < HeaderNames.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = HeaderNames[i];
            }
            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;
            foreach (var m in matches)
            {
                worksheet.Cell(row, 1).Value = m.Id;
                worksheet.Cell(row, 2).Value = m.MatchDate?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty;
                worksheet.Cell(row, 3).Value = m.Stage;
                worksheet.Cell(row, 4).Value = m.Duration?.ToString() ?? string.Empty;
                worksheet.Cell(row, 5).Value = m.TournamentNameNavigation?.Name ?? m.TournamentName;
                worksheet.Cell(row, 6).Value = m.WinnerTeamNavigation?.Name ?? m.WinnerTeam;
                row++;
            }

            workbook.SaveAs(stream);
        }
    }
}

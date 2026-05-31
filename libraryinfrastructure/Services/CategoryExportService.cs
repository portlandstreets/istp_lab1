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
    // Экспорт турнірів у Excel
    public class TournamentExportService : IExportService<Tournament>
    {
        private static readonly IReadOnlyList<string> HeaderNames = new[]
        {
            "Name", "StartDate", "EndDate", "Location", "Organizer", "PrizePool"
        };

        private readonly DbTournamentContext _context;

        public TournamentExportService(DbTournamentContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null || !stream.CanWrite)
                throw new ArgumentException("Stream is not writable", nameof(stream));

            var tournaments = await _context.Tournaments
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Tournaments");

            // Write header
            for (int i = 0; i < HeaderNames.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = HeaderNames[i];
            }
            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;
            foreach (var t in tournaments)
            {
                worksheet.Cell(row, 1).Value = t.Name;
                worksheet.Cell(row, 2).Value = t.StartDate?.ToString("yyyy-MM-dd") ?? string.Empty;
                worksheet.Cell(row, 3).Value = t.EndDate?.ToString("yyyy-MM-dd") ?? string.Empty;
                worksheet.Cell(row, 4).Value = t.Location;
                worksheet.Cell(row, 5).Value = t.OrganizerName;
                worksheet.Cell(row, 6).Value = t.PrizePool;
                row++;
            }

            workbook.SaveAs(stream);
        }
    }
}

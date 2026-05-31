using System;
using tournamentdomain.Model;

namespace libraryinfrastructure.Services
{
    public class MatchDataPortServiceFactory : IDataPortServiceFactory<Match>
    {
        private readonly DbTournamentContext _context;

        public MatchDataPortServiceFactory(DbTournamentContext context)
        {
            _context = context;
        }

        public IImportService<Match> GetImportService(string contentType)
        {
            if (string.Equals(contentType, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", StringComparison.OrdinalIgnoreCase))
            {
                return new MatchImportService(_context);
            }

            if (string.Equals(contentType, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", StringComparison.OrdinalIgnoreCase)
                || string.Equals(contentType, "application/msword", StringComparison.OrdinalIgnoreCase))
            {
                return new MatchDocImportService(_context);
            }

            throw new NotSupportedException($"No import service implemented for matches with content type {contentType}");
        }

        public IExportService<Match> GetExportService(string contentType)
        {
            if (string.Equals(contentType, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", StringComparison.OrdinalIgnoreCase))
            {
                return new MatchExportService(_context);
            }

            throw new NotSupportedException($"No export service implemented for matches with content type {contentType}");
        }
    }
}

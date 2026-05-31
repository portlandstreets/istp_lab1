using System;
using tournamentdomain.Model;

namespace libraryinfrastructure.Services
{
    public class TournamentDataPortServiceFactory : IDataPortServiceFactory<Tournament>
    {
        private readonly DbTournamentContext _context;

        public TournamentDataPortServiceFactory(DbTournamentContext context)
        {
            _context = context;
        }

        public IImportService<Tournament> GetImportService(string contentType)
        {
            // Import for tournaments not implemented yet
            throw new NotSupportedException($"No import service implemented for tournaments with content type {contentType}");
        }

        public IExportService<Tournament> GetExportService(string contentType)
        {
            if (string.Equals(contentType, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", StringComparison.OrdinalIgnoreCase))
            {
                return new TournamentExportService(_context);
            }

            throw new NotSupportedException($"No export service implemented for tournaments with content type {contentType}");
        }
    }
}

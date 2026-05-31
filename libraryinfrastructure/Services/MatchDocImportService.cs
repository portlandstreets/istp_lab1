using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO.Compression;
using tournamentdomain.Model;

namespace libraryinfrastructure.Services
{
    // Imports matches from a .docx Word document.
    // Expected simple format: each paragraph represents one match with fields separated by semicolon ';'
    // Fields order: MatchDate;Stage;Duration;TournamentName;WinnerTeam
    // Example paragraph: 2024-05-01 14:00;Quarterfinal;45;Spring Cup;Team A
    public class MatchDocImportService : IImportService<Match>
    {
        private readonly DbTournamentContext _context;

        public MatchDocImportService(DbTournamentContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null || !stream.CanRead)
                throw new ArgumentException("Stream is not readable", nameof(stream));

            // read docx as zip and extract document.xml
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
            var entry = archive.Entries.FirstOrDefault(e => e.FullName.Equals("word/document.xml", StringComparison.OrdinalIgnoreCase));
            if (entry == null)
                throw new InvalidDataException("The .docx file does not contain document.xml and cannot be parsed.");

            string xml;
            using (var es = entry.Open())
            using (var sr = new StreamReader(es))
            {
                xml = await sr.ReadToEndAsync();
            }

            var xdoc = XDocument.Parse(xml);
            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            // paragraphs
            var paragraphs = xdoc.Descendants(w + "p")
                .Select(p => string.Concat(p.Descendants(w + "t").Select(t => (string)t))).ToList();

            var matchesToAdd = new List<Match>();

            int lineNo = 0;
            foreach (var para in paragraphs)
            {
                lineNo++;
                var text = para?.Trim();
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                // split by semicolon or tab
                var parts = text.Split(new[] { ';', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim()).ToArray();

                if (parts.Length < 5)
                    throw new InvalidDataException($"Invalid format in paragraph {lineNo}. Expected 5 fields separated by ';': MatchDate;Stage;Duration;TournamentName;WinnerTeam");

                // parse date
                if (!DateTime.TryParse(parts[0], out var matchDate))
                    throw new InvalidDataException($"Invalid date in paragraph {lineNo}: '{parts[0]}'");

                var match = new Match
                {
                    MatchDate = matchDate,
                    Stage = parts[1]
                };

                // duration
                var durStr = parts[2];
                if (TimeSpan.TryParse(durStr, out var ts))
                {
                    match.Duration = ts;
                }
                else if (double.TryParse(durStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var minutes))
                {
                    match.Duration = TimeSpan.FromMinutes(minutes);
                }
                else
                {
                    throw new InvalidDataException($"Invalid duration in paragraph {lineNo}: '{durStr}'");
                }

                match.TournamentName = parts[3];
                match.WinnerTeam = parts[4];

                matchesToAdd.Add(match);
            }

            if (matchesToAdd.Any())
            {
                _context.Matches.AddRange(matchesToAdd);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

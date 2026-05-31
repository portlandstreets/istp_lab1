using System;
using System.Collections.Generic;

namespace tournamentdomain.Model;

public partial class Team
{
    public string Name { get; set; } = null!;

    public string? Country { get; set; }

    public int? CreationYear { get; set; }

    public int? RankingPoints { get; set; }

    public int? CoachId { get; set; }

    public virtual Captain? Captain { get; set; }

    public virtual Coach? Coach { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual ICollection<Statistic> Statistics { get; set; } = new List<Statistic>();

    public virtual ICollection<Tournament> TournamentNames { get; set; } = new List<Tournament>();
}

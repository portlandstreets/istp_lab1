using System;
using System.Collections.Generic;

namespace tournamentdomain.Model;

public partial class Match
{
    public int Id { get; set; }

    public DateTime? MatchDate { get; set; }

    public string? Stage { get; set; }

    public TimeSpan? Duration { get; set; }

    public string? WinnerTeam { get; set; }

    public string? TournamentName { get; set; }

    public virtual ICollection<Statistic> Statistics { get; set; } = new List<Statistic>();

    public virtual Tournament? TournamentNameNavigation { get; set; }

    public virtual Team? WinnerTeamNavigation { get; set; }
}

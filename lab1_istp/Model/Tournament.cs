using System;
using System.Collections.Generic;

namespace tournamentdomain.Model;

public partial class Tournament
{
    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? PrizePool { get; set; }

    public string? OrganizerName { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual Organizer? OrganizerNameNavigation { get; set; }

    public virtual ICollection<Team> TeamNames { get; set; } = new List<Team>();
}

using System;
using System.Collections.Generic;

namespace tournamentdomain.Model;

public partial class Statistic
{
    public int MatchId { get; set; }

    public string TeamName { get; set; } = null!;

    public int? Kills { get; set; }

    public int? Deaths { get; set; }

    public int? Damage { get; set; }

    public virtual Match Match { get; set; } = null!;

    public virtual Team TeamNameNavigation { get; set; } = null!;
}

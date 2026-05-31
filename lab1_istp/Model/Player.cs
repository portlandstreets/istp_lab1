using System;
using System.Collections.Generic;

namespace tournamentdomain.Model;

public partial class Player
{
    public string Nickname { get; set; } = null!;

    public string? RealName { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Country { get; set; }

    public string? Role { get; set; }

    public string? TeamName { get; set; }

    public string? SubstituteFor { get; set; }

    public virtual Captain? Captain { get; set; }

    public virtual ICollection<Player> InverseSubstituteForNavigation { get; set; } = new List<Player>();

    public virtual Player? SubstituteForNavigation { get; set; }

    public virtual Team? TeamNameNavigation { get; set; }
}

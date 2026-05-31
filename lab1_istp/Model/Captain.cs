using System;
using System.Collections.Generic;

namespace tournamentdomain.Model;

public partial class Captain
{
    public string Nickname { get; set; } = null!;

    public string? TeamName { get; set; }

    public virtual Player NicknameNavigation { get; set; } = null!;

    public virtual Team? TeamNameNavigation { get; set; }
}

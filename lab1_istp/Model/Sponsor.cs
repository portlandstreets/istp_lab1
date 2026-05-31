using System;
using System.Collections.Generic;

namespace tournamentdomain.Model;

public partial class Sponsor
{
    public string Name { get; set; } = null!;

    public string? Type { get; set; }

    public string? Country { get; set; }

    public string? ContactEmail { get; set; }

    public virtual ICollection<Sponsorship> Sponsorships { get; set; } = new List<Sponsorship>();
}

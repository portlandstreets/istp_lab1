using System;
using System.Collections.Generic;

namespace tournamentdomain.Model;

public partial class Sponsorship
{
    public int Id { get; set; }

    public string? SponsorName { get; set; }

    public string EntityName { get; set; } = null!;

    public string? EntityType { get; set; }

    public decimal? Amount { get; set; }

    public virtual Sponsor? SponsorNameNavigation { get; set; }
}

using System;
using System.Collections.Generic;

namespace tournamentdomain.Model;

public partial class Organizer
{
    public string Name { get; set; } = null!;

    public string? Company { get; set; }

    public string ContactEmail { get; set; } = null!;

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}

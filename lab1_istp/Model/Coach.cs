using System;
using System.Collections.Generic;

namespace tournamentdomain.Model;

public partial class Coach
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public DateOnly? BirthDate { get; set; }

    public int? ExperienceYears { get; set; }

    public decimal? Salary { get; set; }

    public virtual Team? Team { get; set; }
}

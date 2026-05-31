using Microsoft.AspNetCore.Identity;

namespace tournamentdomain.Model
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; } = "";
        public string? LastName { get; set; } = "";
    }
}

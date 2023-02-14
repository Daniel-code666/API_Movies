using Microsoft.AspNetCore.Identity;

namespace API_Movies.Models
{
    public class AppUser : IdentityUser
    {
        public string name { get; set; }
    }
}

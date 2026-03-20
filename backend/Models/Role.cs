using Microsoft.AspNetCore.Identity;

namespace Shopping_Pet.Models
{
    public class Role : IdentityRole<string>
    {
        public string? Description { get; set; }
    }
}

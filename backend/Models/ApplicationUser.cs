using Microsoft.AspNetCore.Identity;

namespace Shopping_Pet.Models
{
    public class ApplicationUser : IdentityUser<string>
    {
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

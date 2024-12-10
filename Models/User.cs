using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Project8.Backend.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        public string Role { get; set; } = "User";
    }
}
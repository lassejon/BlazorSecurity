using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BlazorSecurity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser;

public class CprUser
{
    [Key]
    [Required]
    public string UserId { get; set; }
    
    [Required]
    public string CprNumberHash { get; set; }
    
    public byte[]? PasswordSalt { get; set; }
}
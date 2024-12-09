using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorSecurity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser;

public class CprUser
{
    [Key]
    [Required]
    public string UserId { get; set; }
    
    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "The CPR number must be exactly 10 digits.")]
    public string CprNumber { get; set; }
}
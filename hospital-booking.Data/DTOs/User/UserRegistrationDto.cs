using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace hospital_booking.Data.DTOs.User;

public class UserRegistrationDto
{


    // Corresponds to 'Fullname'
    [Required(ErrorMessage = "Fullname is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Fullname must be between 3 and 50 characters.")]
    [RegularExpression("^[a-zA-Z0-9._]+$", ErrorMessage = "Fullname can only contain letters, numbers, dots, and underscores.")]
    public string Fullname { get; set; } = string.Empty;

    // Corresponds to 'email'
    [Required(ErrorMessage = "Email is required.")]
    [StringLength(320, ErrorMessage = "Email cannot exceed 320 characters.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;


    // Password field for user input.
    // The hash is generated in the backend, so this property holds the plain text password.
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = string.Empty;


}


using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Patient
{
    public class PatientGoogleLoginDto
    {
        [Required(ErrorMessage = "Google ID token is required")]
        public string IdToken { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hospital_booking.Data.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("fullname")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(320)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;


        [StringLength(255)]
        [Column("password")]
        public string? Password { get; set; }

    }
}
using System.ComponentModel.DataAnnotations;

namespace TodoWeb.Application.Dtos.StudentModel
{
    public class StudentCreateViewModel
    {
        [Required]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public int SchoolId { get; set; }

        public decimal Balance { get; set; } = 0;

        [StringLength(500)]
        public string? Address1 { get; set; }

        [StringLength(500)]
        public string? Address2 { get; set; }

        [StringLength(500)]
        public string? Address3 { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}

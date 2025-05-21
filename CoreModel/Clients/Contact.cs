using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACIA.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public short ContactCode { get; set; }

        [StringLength(20)]
        public string PersonalId { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(20)]
        public string? CellNumber { get; set; }

        [StringLength(20)]
        public string? FaxNumber { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Position { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(250)]
        public string? Notes { get; set; }

        public DateTime? OpeningEffecDate { get; set; }

        public DateTime? ClosingEffecDate { get; set; }

        public DateTime? OpeningRegDate { get; set; }

        public DateTime? ClosingRegDate { get; set; }

        public int? OpeningRef { get; set; }

        public int? ClosingRef { get; set; }

        [Required]
        public bool ActiveFlag { get; set; } = true;

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        // Helper property to get full name
        [NotMapped]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                    return string.Empty;
                else if (string.IsNullOrEmpty(FirstName))
                    return LastName;
                else if (string.IsNullOrEmpty(LastName))
                    return FirstName;
                else
                    return $"{FirstName} {LastName}";
            }
        }
    }
}
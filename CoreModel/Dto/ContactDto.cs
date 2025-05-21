using System;
using System.ComponentModel.DataAnnotations;

namespace ACIA.DTOs
{
    public class ContactDto
    {
        // ID = 0 means new contact
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public string? CompanyName { get; set; }

        public short ContactCode { get; set; }

        [StringLength(20)]
        public string? PersonalId { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        public string? Name { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(20)]
        public string? CellNumber { get; set; }

        [StringLength(20)]
        public string? FaxNumber { get; set; }

        [StringLength(255)]
        [EmailAddress]
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

        public bool? ActiveFlag { get; set; } = true;
    }
}
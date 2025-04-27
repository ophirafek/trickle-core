using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeadManagerPro.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(50)]
        public string? Size { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }

        [StringLength(50)]
        public string? Status { get; set; } = "Active";

        // Address information
        [StringLength(200)]
        public string? StreetAddress { get; set; }

        [StringLength(50)]
        public string? Suite { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? StateProvince { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        // Billing Address (if different)
        [StringLength(200)]
        public string? BillingStreet { get; set; }

        [StringLength(100)]
        public string? BillingCity { get; set; }

        [StringLength(20)]
        public string? BillingPostalCode { get; set; }

        // Additional information
        [StringLength(255)]
        public string? LinkedInProfile { get; set; }

        public int? FoundingYear { get; set; }

        public string? Description { get; set; }
        [StringLength(50)]
        public string? RegistrationNumber { get; set; }

        [StringLength(50)]
        public string? DunsNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<Note> Notes { get; set; }
    }

    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string JobTitle { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }

    public class Note
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
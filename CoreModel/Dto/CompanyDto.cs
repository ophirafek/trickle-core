using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeadManagerPro.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Industry { get; set; }
        public string Size { get; set; }
        public string Location { get; set; }
        public string Website { get; set; }
        public string Status { get; set; }
        public string StreetAddress { get; set; }
        public string Suite { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string BillingStreet { get; set; }
        public string BillingCity { get; set; }
        public string BillingPostalCode { get; set; }
        public string LinkedInProfile { get; set; }
        public int? FoundingYear { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<ContactDto> Contacts { get; set; }
        public ICollection<NoteDto> Notes { get; set; }
    }

    public class CompanyCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Industry { get; set; }

        [StringLength(50)]
        public string Size { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        [StringLength(255)]
        public string Website { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active";

        // Address information
        [StringLength(200)]
        public string StreetAddress { get; set; }

        [StringLength(50)]
        public string Suite { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string StateProvince { get; set; }

        [StringLength(20)]
        public string PostalCode { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        // Billing Address
        [StringLength(200)]
        public string BillingStreet { get; set; }

        [StringLength(100)]
        public string BillingCity { get; set; }

        [StringLength(20)]
        public string BillingPostalCode { get; set; }

        // Additional information
        [StringLength(255)]
        public string LinkedInProfile { get; set; }

        public int? FoundingYear { get; set; }

        public string Description { get; set; }
    }

    public class CompanyUpdateDto : CompanyCreateDto
    {
        // Uses all properties from CompanyCreateDto
    }

    public class ContactDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CompanyId { get; set; }
    }

    public class ContactCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string JobTitle { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }

    public class ContactUpdateDto : ContactCreateDto
    {
        // Uses all properties from ContactCreateDto
    }

    public class NoteDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CompanyId { get; set; }
    }

    public class NoteCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }

    public class NoteUpdateDto : NoteCreateDto
    {
        // Uses all properties from NoteCreateDto
    }
}
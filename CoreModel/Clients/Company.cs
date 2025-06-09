using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACIA.Models
{
    public class Company
    {
        public int Id { get; set; }
        public int IdTypeCode { get; set; }
        [Required]
        [StringLength(100)]
        public string RegistrationNumber { get; set; }  // Added field
        public string? DunsNumber { get; set; }         // Added field
        public string? VATNumber { get; set; }         // Added field

        [Required]
        [StringLength(100)]
        public string RegistrationName { get; set; }

        [StringLength(100)]
        public string TradeName { get; set; }
        [StringLength(100)]
        public string EnglishName { get; set; }
        public int CompanyStatusCode { get; set; }
        public int BusinessFieldCode { get; set; }
        public int OrganizationTypeCode { get; set; }
        public int? FoundingYear { get; set; }
        public int CountryCode { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }


        [StringLength(200)]
        public string? StreetAddress { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? StateProvince { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? FaxNumber { get; set; }
        public string? EMailAddress { get; set; }

        public string? Remarks { get; set; }

        public DateTime? LastReportDate { get; set; }
        public string? LastReportName { get; set; }

        public DateTime? OpeningEffecDate { get; set; }
        public DateTime? ClosingEffecDate { get; set; }
        public DateTime? OpeningRegDate { get; set; }
        public DateTime? ClosingRegDate { get; set; }
        public string? OpeningRef { get; set; }
        public string? ClosingRef { get; set; }
        public bool ActiveFlag { get; set; } = true; // Default to true

        // Navigation properties
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<Note> Notes { get; set; }
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
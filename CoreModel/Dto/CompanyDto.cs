using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACIA.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string RegistrationNumber { get; set; }  // Added field
        public string? DunsNumber { get; set; }         // Added field
        public string? VATNumber { get; set; }         // Added field
        public int? IdTypeCode { get; set; }
        [Required]
        [StringLength(100)]
        public string RegistrationName { get; set; }

        [StringLength(100)]
        public string? TradeName { get; set; }
        [StringLength(100)]
        public string? EnglishName { get; set; }
        public int? CompanyStatusCode { get; set; }
        public int? BusinessFieldCode { get; set; }
        public int? EntityTypeCode { get; set; }
        public int? FoundingYear { get; set; }
        public int? CountryCode { get; set; }

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
        
        public string? Remarks{ get; set; }

        public DateTime? LastReportDate { get; set; }
        public string? LastReportName { get; set; }

        public DateTime? OpeningEffectiveDate { get; set; }
        public DateTime? ClosingEffectiveDate { get; set; }
        public DateTime? OpeningRegDate { get; set; }
        public DateTime? ClosingRegDate { get; set; }
        public string? OpeningRef{ get; set; }
        public string? ClosingRef{ get; set; }
        public int? AssignedTeamMemberId { get; set; }
        public string? AssignedTeamMemberName { get; set; }
        public InsuredCompanyDto? InsuredDetails { get; set; }
        public bool IsInsured { get;set; }
        public bool IsDebtor { get; set; }
        public bool IsPotentialInsured { get; set; }

        public ICollection<ContactDto>? Contacts { get; set; }

        public ICollection<NoteDto>? Notes { get; set; }
        public bool IsActive { get; set; }
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
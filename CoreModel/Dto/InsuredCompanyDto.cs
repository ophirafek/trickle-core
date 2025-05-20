// CoreModel/Dto/InsuredCompanyDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ACIA.DTOs
{
    public class InsuredCompanyDto
    {
        public int ID { get; set; } // ID = 0 means new insured company record

        [Required]
        public int CompanyID { get; set; }

        public string? CompanyName { get; set; }

        public short? StatusCode { get; set; }

        public short? SizeCode { get; set; }

        public DateTime? InsuranceEntryDate { get; set; }

        public DateTime? OpeningEffecDate { get; set; }

        public DateTime? ClosingEffecDate { get; set; }

        public DateTime? OpeningRegDate { get; set; }

        public DateTime? ClosingRegDate { get; set; }

        public int? OpeningRef { get; set; }

        public int? ClosingRef { get; set; }

        public short? ActiveFlag { get; set; } = 1;
    }
}
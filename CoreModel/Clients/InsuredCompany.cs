// CoreModel/Clients/InsuredCompany.cs
using ACIA.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACIA.Models
{
    public class InsuredCompany
    {
        public int ID { get; set; }

        [Required]
        public int CompanyID { get; set; }

        public short StatusCode { get; set; }

        public short SizeCode { get; set; }

        public DateTime? InsuranceEntryDate { get; set; }

        public DateTime? OpeningEffecDate { get; set; }

        public DateTime? ClosingEffecDate { get; set; }

        public DateTime? OpeningRegDate { get; set; }

        public DateTime? ClosingRegDate { get; set; }

        public int? OpeningRef { get; set; }

        public int? ClosingRef { get; set; }

        [Required]
        public short ActiveFlag { get; set; } = 1;

        [ForeignKey("CompanyID")]
        public Company Company { get; set; }
    }
}
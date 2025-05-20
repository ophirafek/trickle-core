// CoreModel/Clients/Assignment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACIA.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [Required]
        public int AssignmentTypeCode { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        public DateTime? OpeningEffecDate { get; set; }

        public DateTime? ClosingEffecDate { get; set; }

        public DateTime? OpeningRegDate { get; set; }

        public DateTime? ClosingRegDate { get; set; }

        public int? OpeningRef { get; set; }

        public int? ClosingRef { get; set; }

        [Required]
        public int ActiveFlag { get; set; } = 1;

        [ForeignKey("CompanyID")]
        public Company Company { get; set; }
    }
}
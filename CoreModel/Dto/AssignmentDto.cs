// CoreModel/Dto/AssignmentDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ACIA.DTOs
{
    public class AssignmentDto
    {
        public int Id { get; set; } // ID = 0 means new assignment

        [Required]
        public int CompanyID { get; set; }

        public string? CompanyName { get; set; }

        [Required]
        public int AssignmentTypeCode { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        public string? EmployeeName { get; set; }

        public DateTime? OpeningEffecDate { get; set; }

        public DateTime? ClosingEffecDate { get; set; }

        public DateTime? OpeningRegDate { get; set; }

        public DateTime? ClosingRegDate { get; set; }

        public int? OpeningRef { get; set; }

        public int? ClosingRef { get; set; }

        [Required]
        public int ActiveFlag { get; set; } = 1;
    }
}
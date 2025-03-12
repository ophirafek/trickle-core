using System;
using System.ComponentModel.DataAnnotations;

namespace LeadManagerPro.DTOs
{
    public class LeadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public decimal Value { get; set; }
        public int Probability { get; set; }
        public string Owner { get; set; }
        public string Source { get; set; }
        public DateTime? ExpectedCloseDate { get; set; }
        public string Description { get; set; }
        public string NextSteps { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class LeadCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "New";

        [Range(0, double.MaxValue)]
        public decimal Value { get; set; }

        [Range(0, 100)]
        public int Probability { get; set; }

        [StringLength(100)]
        public string Owner { get; set; }

        [StringLength(50)]
        public string Source { get; set; }

        public DateTime? ExpectedCloseDate { get; set; }

        public string Description { get; set; }

        public string NextSteps { get; set; }
    }

    public class LeadUpdateDto : LeadCreateDto
    {
        // Uses all properties from LeadCreateDto
    }
}
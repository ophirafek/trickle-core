using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeadManagerPro.Models
{
    public class Lead
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
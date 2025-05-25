// CoreModel/Clients/Lead.cs - Updated to match new database structure
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACIA.Models
{
    public class Lead
    {
        public int LeadId { get; set; }

        [Required]
        [StringLength(255)]
        public string LeadName { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public int? LeadTypeCode { get; set; }

        public int? LeadSourceCode { get; set; }

        public int? ContactId { get; set; }

        public int? CurrencyCode { get; set; }

        public int? MarketCode { get; set; }

        public int? AgentId { get; set; }

        public int? OwnerEmployeeId { get; set; }

        [Range(0, 100)]
        public int? Probability { get; set; }

        [Range(0, 100)]
        public int? Score { get; set; }

        public int? Employees { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ActualSalesValue { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SalesGapValue { get; set; }

        public string? ActivityExpansion { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ExportMarketValue { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? LocalMarketValue { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ExportRatio { get; set; }

        [StringLength(255)]
        public string? Region { get; set; }

        [StringLength(255)]
        public string? CurrentInsurerNo { get; set; }

        public DateTime? ExternalStartDate { get; set; }

        [StringLength(50)]
        public string? StatusCode { get; set; }

        public int? ReasonRejectionCode { get; set; }

        public string? RejectionDetail { get; set; }

        [StringLength(250)]
        public string? Notes { get; set; }

        public string? AdditionalInfo { get; set; }

        public DateTime? OpeningEffectiveDate { get; set; }

        public DateTime? ClosingEffectiveDate { get; set; }

        public DateTime? OpeningRegistrationDate { get; set; }

        public DateTime? ClosingRegistrationDate { get; set; }

        public int? OpeningReference { get; set; }

        public int? ClosingReference { get; set; }

        public bool ActiveFlag { get; set; } = true;

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        // Navigation property for Contact if needed
        [ForeignKey("ContactId")]
        public Contact? Contact { get; set; }

        // Computed properties for backward compatibility or display
        [NotMapped]
        public string Title => LeadName;

        [NotMapped]
        public decimal Value => SalesGapValue ?? 0;

        [NotMapped]
        public string Status => StatusCode ?? "New";

        [NotMapped]
        public DateTime CreatedAt => OpeningRegistrationDate ?? DateTime.UtcNow;

        [NotMapped]
        public DateTime? UpdatedAt => ClosingRegistrationDate;
    }
}
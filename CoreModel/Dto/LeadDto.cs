// CoreModel/Dto/LeadDto.cs - Updated to match new database structure
using System;
using System.ComponentModel.DataAnnotations;

namespace ACIA.DTOs
{
    public class LeadDto
    {
        public int LeadId { get; set; }

        [Required]
        [StringLength(255)]
        public string LeadName { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public string? CompanyName { get; set; }

        public int? LeadTypeCode { get; set; }
        public string? LeadTypeName { get; set; }

        public int? LeadSourceCode { get; set; }
        public string? LeadSourceName { get; set; }

        public int? ContactId { get; set; }
        public string? ContactName { get; set; }

        public int? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        public int? MarketCode { get; set; }
        public string? MarketName { get; set; }

        public int? AgentId { get; set; }
        public string? AgentName { get; set; }

        public int? OwnerEmployeeId { get; set; }
        public string? OwnerName { get; set; }

        [Range(0, 100)]
        public int? Probability { get; set; }

        [Range(0, 100)]
        public int? Score { get; set; }

        public int? Employees { get; set; }

        public decimal? ActualSalesValue { get; set; }

        public decimal? SalesGapValue { get; set; }

        public string? ActivityExpansion { get; set; }

        public decimal? ExportMarketValue { get; set; }

        public decimal? LocalMarketValue { get; set; }

        public decimal? ExportRatio { get; set; }

        public string? Region { get; set; }

        public string? CurrentInsurerNo { get; set; }

        public DateTime? ExternalStartDate { get; set; }

        public string? StatusCode { get; set; }
        public string? StatusName { get; set; }

        public int? ReasonRejectionCode { get; set; }
        public string? ReasonRejectionName { get; set; }

        public string? RejectionDetail { get; set; }

        public string? Notes { get; set; }

        public string? AdditionalInfo { get; set; }

        public DateTime? OpeningEffectiveDate { get; set; }

        public DateTime? ClosingEffectiveDate { get; set; }

        public DateTime? OpeningRegistrationDate { get; set; }

        public DateTime? ClosingRegistrationDate { get; set; }

        public int? OpeningReference { get; set; }

        public int? ClosingReference { get; set; }

        public bool ActiveFlag { get; set; } = true;

        // Backward compatibility properties
        public int Id => LeadId;
        public string Title => LeadName;
        public decimal Value => SalesGapValue ?? 0;
        public string Status => StatusCode ?? "New";
        public string Owner => OwnerName ?? "";
        public DateTime CreatedAt => OpeningRegistrationDate ?? DateTime.UtcNow;
        public DateTime? UpdatedAt => ClosingRegistrationDate;
    }

    public class LeadCreateDto
    {
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

        public decimal? ActualSalesValue { get; set; }

        public decimal? SalesGapValue { get; set; }

        public string? ActivityExpansion { get; set; }

        public decimal? ExportMarketValue { get; set; }

        public decimal? LocalMarketValue { get; set; }

        public decimal? ExportRatio { get; set; }

        public string? Region { get; set; }

        public string? CurrentInsurerNo { get; set; }

        public DateTime? ExternalStartDate { get; set; }

        public string? StatusCode { get; set; } = "New";

        public int? ReasonRejectionCode { get; set; }

        public string? RejectionDetail { get; set; }

        public string? Notes { get; set; }

        public string? AdditionalInfo { get; set; }

        // Backward compatibility properties
        [Required]
        public string Title
        {
            get => LeadName;
            set => LeadName = value;
        }

        public decimal Value
        {
            get => SalesGapValue ?? 0;
            set => SalesGapValue = value;
        }

        public string Status
        {
            get => StatusCode ?? "New";
            set => StatusCode = value;
        }

        public string? Owner { get; set; } // This would need to be resolved to OwnerEmployeeId

        public string? Source { get; set; } // This would need to be resolved to LeadSourceCode

        public DateTime? ExpectedCloseDate { get; set; } // Could map to ExternalStartDate

        public string? Description
        {
            get => Notes;
            set => Notes = value;
        }

        public string? NextSteps
        {
            get => AdditionalInfo;
            set => AdditionalInfo = value;
        }
    }

    public class LeadUpdateDto : LeadCreateDto
    {
        public int LeadId { get; set; }

        public DateTime? ClosingEffectiveDate { get; set; }

        public DateTime? ClosingRegistrationDate { get; set; }

        public int? ClosingReference { get; set; }

        public bool ActiveFlag { get; set; } = true;
    }
}
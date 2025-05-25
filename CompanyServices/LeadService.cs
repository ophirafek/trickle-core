// CompanyServices/LeadService.cs - Updated for new lead structure
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
using ACIA.Data;
using ACIA.DTOs;
using ACIA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ACIA.Services
{
    public class LeadService : ILeadService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LeadService> _logger;
        private readonly IIdPoolService _poolService;

        public LeadService(ApplicationDbContext context, ILogger<LeadService> logger, IIdPoolService poolService)
        {
            _context = context;
            _logger = logger;
            _poolService = poolService;
        }

        public async Task<IEnumerable<LeadDto>> GetLeadsAsync()
        {
            try
            {
                var leads = await _context.Leads
                    .Include(l => l.Company)
                    .Include(l => l.Contact)
                    .Where(l => l.ActiveFlag)
                    .Select(l => MapToLeadDto(l))
                    .ToListAsync();

                return leads;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting leads");
                throw;
            }
        }

        public async Task<IEnumerable<LeadDto>> GetLeadsByCompanyAsync(int companyId)
        {
            try
            {
                var leads = await _context.Leads
                    .Include(l => l.Company)
                    .Include(l => l.Contact)
                    .Where(l => l.CompanyId == companyId && l.ActiveFlag)
                    .Select(l => MapToLeadDto(l))
                    .ToListAsync();

                return leads;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting leads for company {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<LeadDto> GetLeadByIdAsync(int id)
        {
            try
            {
                var lead = await _context.Leads
                    .Include(l => l.Company)
                    .Include(l => l.Contact)
                    .FirstOrDefaultAsync(l => l.LeadId == id && l.ActiveFlag);

                if (lead == null)
                {
                    return null;
                }

                return MapToLeadDto(lead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting lead with id {Id}", id);
                throw;
            }
        }

        public async Task<LeadDto> CreateLeadAsync(LeadCreateDto leadDto)
        {
            try
            {
                // Verify the company exists
                var companyExists = await _context.Companies.AnyAsync(c => c.Id == leadDto.CompanyId);
                if (!companyExists)
                {
                    throw new KeyNotFoundException($"Company with ID {leadDto.CompanyId} not found");
                }

                // Verify contact exists if provided
                if (leadDto.ContactId.HasValue)
                {
                    var contactExists = await _context.Contacts.AnyAsync(c => c.Id == leadDto.ContactId.Value && c.CompanyId == leadDto.CompanyId);
                    if (!contactExists)
                    {
                        throw new KeyNotFoundException($"Contact with ID {leadDto.ContactId} not found for company {leadDto.CompanyId}");
                    }
                }

                var lead = new Lead
                {
                    LeadId = await _poolService.GetNextIdAsync(IIdPoolService.IdPoolType.Lead),
                    LeadName = leadDto.LeadName,
                    CompanyId = leadDto.CompanyId,
                    LeadTypeCode = leadDto.LeadTypeCode,
                    LeadSourceCode = leadDto.LeadSourceCode,
                    ContactId = leadDto.ContactId,
                    CurrencyCode = leadDto.CurrencyCode,
                    MarketCode = leadDto.MarketCode,
                    AgentId = leadDto.AgentId,
                    OwnerEmployeeId = leadDto.OwnerEmployeeId,
                    Probability = leadDto.Probability,
                    Score = leadDto.Score,
                    Employees = leadDto.Employees,
                    ActualSalesValue = leadDto.ActualSalesValue,
                    SalesGapValue = leadDto.SalesGapValue,
                    ActivityExpansion = leadDto.ActivityExpansion,
                    ExportMarketValue = leadDto.ExportMarketValue,
                    LocalMarketValue = leadDto.LocalMarketValue,
                    ExportRatio = leadDto.ExportRatio,
                    Region = leadDto.Region,
                    CurrentInsurerNo = leadDto.CurrentInsurerNo,
                    ExternalStartDate = leadDto.ExternalStartDate,
                    StatusCode = leadDto.StatusCode ?? "New",
                    ReasonRejectionCode = leadDto.ReasonRejectionCode,
                    RejectionDetail = leadDto.RejectionDetail,
                    Notes = leadDto.Notes,
                    AdditionalInfo = leadDto.AdditionalInfo,
                    OpeningEffectiveDate = DateTime.UtcNow.Date,
                    ClosingEffectiveDate = DateTime.MaxValue,
                    OpeningRegistrationDate = DateTime.UtcNow,
                    ClosingRegistrationDate = DateTime.MaxValue,
                    OpeningReference = 0, // Could be user ID
                    ClosingReference = null,
                    ActiveFlag = true
                };

                _context.Leads.Add(lead);
                await _context.SaveChangesAsync();

                // Return the created lead as DTO
                var createdLead = await GetLeadByIdAsync(lead.LeadId);
                return createdLead;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating lead");
                throw;
            }
        }

        public async Task UpdateLeadAsync(int id, LeadUpdateDto leadDto)
        {
            try
            {
                var existingLead = await _context.Leads.FirstOrDefaultAsync(l => l.LeadId == id && l.ActiveFlag);
                if (existingLead == null)
                {
                    throw new KeyNotFoundException($"Lead with ID {id} not found");
                }

                // Verify the company exists if it's being changed
                if (existingLead.CompanyId != leadDto.CompanyId)
                {
                    var companyExists = await _context.Companies.AnyAsync(c => c.Id == leadDto.CompanyId);
                    if (!companyExists)
                    {
                        throw new KeyNotFoundException($"Company with ID {leadDto.CompanyId} not found");
                    }
                }

                // Verify contact exists if provided
                if (leadDto.ContactId.HasValue)
                {
                    var contactExists = await _context.Contacts.AnyAsync(c => c.Id == leadDto.ContactId.Value && c.CompanyId == leadDto.CompanyId);
                    if (!contactExists)
                    {
                        throw new KeyNotFoundException($"Contact with ID {leadDto.ContactId} not found for company {leadDto.CompanyId}");
                    }
                }

                // Deactivate the current lead
                existingLead.ActiveFlag = false;
                existingLead.ClosingEffectiveDate = DateTime.UtcNow.Date;
                existingLead.ClosingRegistrationDate = DateTime.UtcNow;
                existingLead.ClosingReference = 0; // Could be user ID

                _context.Entry(existingLead).State = EntityState.Modified;

                // Create new lead record with updated data
                var newLead = new Lead
                {
                    LeadId = await _poolService.GetNextIdAsync(IIdPoolService.IdPoolType.Lead),
                    LeadName = leadDto.LeadName,
                    CompanyId = leadDto.CompanyId,
                    LeadTypeCode = leadDto.LeadTypeCode,
                    LeadSourceCode = leadDto.LeadSourceCode,
                    ContactId = leadDto.ContactId,
                    CurrencyCode = leadDto.CurrencyCode,
                    MarketCode = leadDto.MarketCode,
                    AgentId = leadDto.AgentId,
                    OwnerEmployeeId = leadDto.OwnerEmployeeId,
                    Probability = leadDto.Probability,
                    Score = leadDto.Score,
                    Employees = leadDto.Employees,
                    ActualSalesValue = leadDto.ActualSalesValue,
                    SalesGapValue = leadDto.SalesGapValue,
                    ActivityExpansion = leadDto.ActivityExpansion,
                    ExportMarketValue = leadDto.ExportMarketValue,
                    LocalMarketValue = leadDto.LocalMarketValue,
                    ExportRatio = leadDto.ExportRatio,
                    Region = leadDto.Region,
                    CurrentInsurerNo = leadDto.CurrentInsurerNo,
                    ExternalStartDate = leadDto.ExternalStartDate,
                    StatusCode = leadDto.StatusCode,
                    ReasonRejectionCode = leadDto.ReasonRejectionCode,
                    RejectionDetail = leadDto.RejectionDetail,
                    Notes = leadDto.Notes,
                    AdditionalInfo = leadDto.AdditionalInfo,
                    OpeningEffectiveDate = DateTime.UtcNow.Date,
                    ClosingEffectiveDate = DateTime.MaxValue,
                    OpeningRegistrationDate = DateTime.UtcNow,
                    ClosingRegistrationDate = DateTime.MaxValue,
                    OpeningReference = existingLead.LeadId, // Reference to previous version
                    ClosingReference = null,
                    ActiveFlag = true
                };

                _context.Leads.Add(newLead);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating lead with id {Id}", id);
                throw;
            }
        }

        public async Task DeleteLeadAsync(int id)
        {
            try
            {
                var lead = await _context.Leads.FirstOrDefaultAsync(l => l.LeadId == id && l.ActiveFlag);
                if (lead == null)
                {
                    throw new KeyNotFoundException($"Lead with ID {id} not found");
                }

                // Soft delete - mark as inactive
                lead.ActiveFlag = false;
                lead.ClosingEffectiveDate = DateTime.UtcNow.Date;
                lead.ClosingRegistrationDate = DateTime.UtcNow;
                lead.ClosingReference = 0; // Could be user ID

                _context.Entry(lead).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting lead with id {Id}", id);
                throw;
            }
        }

        public async Task<bool> LeadExistsAsync(int id)
        {
            return await _context.Leads.AnyAsync(e => e.LeadId == id && e.ActiveFlag);
        }

        public async Task<IEnumerable<LeadDto>> GetLeadsByOwnerAsync(int ownerEmployeeId)
        {
            try
            {
                var leads = await _context.Leads
                    .Include(l => l.Company)
                    .Include(l => l.Contact)
                    .Where(l => l.OwnerEmployeeId == ownerEmployeeId && l.ActiveFlag)
                    .Select(l => MapToLeadDto(l))
                    .ToListAsync();

                return leads;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting leads for owner {OwnerEmployeeId}", ownerEmployeeId);
                throw;
            }
        }

        public async Task<IEnumerable<LeadDto>> GetLeadsByStatusAsync(string statusCode)
        {
            try
            {
                var leads = await _context.Leads
                    .Include(l => l.Company)
                    .Include(l => l.Contact)
                    .Where(l => l.StatusCode == statusCode && l.ActiveFlag)
                    .Select(l => MapToLeadDto(l))
                    .ToListAsync();

                return leads;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting leads with status {StatusCode}", statusCode);
                throw;
            }
        }

        public async Task<IEnumerable<LeadDto>> GetLeadsByMarketAsync(int marketCode)
        {
            try
            {
                var leads = await _context.Leads
                    .Include(l => l.Company)
                    .Include(l => l.Contact)
                    .Where(l => l.MarketCode == marketCode && l.ActiveFlag)
                    .Select(l => MapToLeadDto(l))
                    .ToListAsync();

                return leads;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting leads for market {MarketCode}", marketCode);
                throw;
            }
        }

        public async Task<IEnumerable<LeadDto>> GetLeadsByContactAsync(int contactId)
        {
            try
            {
                var leads = await _context.Leads
                    .Include(l => l.Company)
                    .Include(l => l.Contact)
                    .Where(l => l.ContactId == contactId && l.ActiveFlag)
                    .Select(l => MapToLeadDto(l))
                    .ToListAsync();

                return leads;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting leads for contact {ContactId}", contactId);
                throw;
            }
        }

        private static LeadDto MapToLeadDto(Lead lead)
        {
            var contactName = lead.Contact != null
                ? $"{lead.Contact.FirstName} {lead.Contact.LastName}".Trim()
                : null;

            return new LeadDto
            {
                LeadId = lead.LeadId,
                LeadName = lead.LeadName,
                CompanyId = lead.CompanyId,
                CompanyName = lead.Company?.RegistrationName,
                LeadTypeCode = lead.LeadTypeCode,
                LeadSourceCode = lead.LeadSourceCode,
                ContactId = lead.ContactId,
                ContactName = contactName,
                CurrencyCode = lead.CurrencyCode,
                MarketCode = lead.MarketCode,
                AgentId = lead.AgentId,
                OwnerEmployeeId = lead.OwnerEmployeeId,
                Probability = lead.Probability,
                Score = lead.Score,
                Employees = lead.Employees,
                ActualSalesValue = lead.ActualSalesValue,
                SalesGapValue = lead.SalesGapValue,
                ActivityExpansion = lead.ActivityExpansion,
                ExportMarketValue = lead.ExportMarketValue,
                LocalMarketValue = lead.LocalMarketValue,
                ExportRatio = lead.ExportRatio,
                Region = lead.Region,
                CurrentInsurerNo = lead.CurrentInsurerNo,
                ExternalStartDate = lead.ExternalStartDate,
                StatusCode = lead.StatusCode,
                ReasonRejectionCode = lead.ReasonRejectionCode,
                RejectionDetail = lead.RejectionDetail,
                Notes = lead.Notes,
                AdditionalInfo = lead.AdditionalInfo,
                OpeningEffectiveDate = lead.OpeningEffectiveDate,
                ClosingEffectiveDate = lead.ClosingEffectiveDate,
                OpeningRegistrationDate = lead.OpeningRegistrationDate,
                ClosingRegistrationDate = lead.ClosingRegistrationDate,
                OpeningReference = lead.OpeningReference,
                ClosingReference = lead.ClosingReference,
                ActiveFlag = lead.ActiveFlag
            };
        }
    }
}
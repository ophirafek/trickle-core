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
            this._poolService = poolService;
        }

        public async Task<IEnumerable<LeadDto>> GetLeadsAsync()
        {
            try
            {
                var leads = await _context.Leads
                    .Include(l => l.Company)
                    .Select(l => new LeadDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        CompanyId = l.CompanyId,
                        CompanyName = l.Company.RegistrationName,
                        Status = l.Status,
                        Value = l.Value,
                        Probability = l.Probability,
                        Owner = l.Owner,
                        Source = l.Source,
                        ExpectedCloseDate = l.ExpectedCloseDate,
                        Description = l.Description,
                        NextSteps = l.NextSteps,
                        CreatedAt = l.CreatedAt,
                        UpdatedAt = l.UpdatedAt
                    })
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
                    .Where(l => l.CompanyId == companyId)
                    .Select(l => new LeadDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        CompanyId = l.CompanyId,
                        CompanyName = l.Company.RegistrationName,
                        Status = l.Status,
                        Value = l.Value,
                        Probability = l.Probability,
                        Owner = l.Owner,
                        Source = l.Source,
                        ExpectedCloseDate = l.ExpectedCloseDate,
                        Description = l.Description,
                        NextSteps = l.NextSteps,
                        CreatedAt = l.CreatedAt,
                        UpdatedAt = l.UpdatedAt
                    })
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
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (lead == null)
                {
                    return null;
                }

                var leadDto = new LeadDto
                {
                    Id = lead.Id,
                    Title = lead.Title,
                    CompanyId = lead.CompanyId,
                    CompanyName = lead.Company.RegistrationName,
                    Status = lead.Status,
                    Value = lead.Value,
                    Probability = lead.Probability,
                    Owner = lead.Owner,
                    Source = lead.Source,
                    ExpectedCloseDate = lead.ExpectedCloseDate,
                    Description = lead.Description,
                    NextSteps = lead.NextSteps,
                    CreatedAt = lead.CreatedAt,
                    UpdatedAt = lead.UpdatedAt
                };

                return leadDto;
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

                var lead = new Lead
                {
                    Id = await _poolService.GetNextIdAsync(IIdPoolService.IdPoolType.Lead),
                    Title = leadDto.Title,
                    CompanyId = leadDto.CompanyId,
                    Status = leadDto.Status,
                    Value = leadDto.Value,
                    Probability = leadDto.Probability,
                    Owner = leadDto.Owner ?? "",
                    Source = leadDto.Source ?? "",
                    ExpectedCloseDate = leadDto.ExpectedCloseDate ?? DateTime.UtcNow,
                    Description = leadDto.Description ?? "",
                    NextSteps = leadDto.NextSteps ?? "",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Leads.Add(lead);
                await _context.SaveChangesAsync();

                // Get the company name for the response
                var company = await _context.Companies.FindAsync(leadDto.CompanyId);

                var newLeadDto = new LeadDto
                {
                    Id = lead.Id,
                    Title = lead.Title,
                    CompanyId = lead.CompanyId,
                    CompanyName = company.RegistrationName,
                    Status = lead.Status,
                    Value = lead.Value,
                    Probability = lead.Probability,
                    Owner = lead.Owner,
                    Source = lead.Source,
                    ExpectedCloseDate = lead.ExpectedCloseDate,
                    Description = lead.Description,
                    NextSteps = lead.NextSteps,
                    CreatedAt = lead.CreatedAt,
                    UpdatedAt = lead.UpdatedAt
                };

                return newLeadDto;
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
                var lead = await _context.Leads.FindAsync(id);
                if (lead == null)
                {
                    throw new KeyNotFoundException($"Lead with ID {id} not found");
                }

                // Verify the company exists if it's being changed
                if (lead.CompanyId != leadDto.CompanyId)
                {
                    var companyExists = await _context.Companies.AnyAsync(c => c.Id == leadDto.CompanyId);
                    if (!companyExists)
                    {
                        throw new KeyNotFoundException($"Company with ID {leadDto.CompanyId} not found");
                    }
                }

                // Update lead properties
                lead.Title = leadDto.Title;
                lead.CompanyId = leadDto.CompanyId;
                lead.Status = leadDto.Status;
                lead.Value = leadDto.Value;
                lead.Probability = leadDto.Probability;
                lead.Owner = leadDto.Owner;
                lead.Source = leadDto.Source;
                lead.ExpectedCloseDate = leadDto.ExpectedCloseDate;
                lead.Description = leadDto.Description;
                lead.NextSteps = leadDto.NextSteps;
                lead.UpdatedAt = DateTime.UtcNow;

                _context.Entry(lead).State = EntityState.Modified;
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
                var lead = await _context.Leads.FindAsync(id);
                if (lead == null)
                {
                    throw new KeyNotFoundException($"Lead with ID {id} not found");
                }

                _context.Leads.Remove(lead);
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
            return await _context.Leads.AnyAsync(e => e.Id == id);
        }
    }
}
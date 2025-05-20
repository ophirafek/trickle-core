// CompanyServices/InsuredCompanyService.cs
using System;
using System.Threading.Tasks;
using ACIA.Data;
using ACIA.DTOs;
using ACIA.Models;
using ACIA.Services;
using Base;
using Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Interfaces.IIdPoolService;

namespace ACIA.Services
{
    public class InsuredCompanyService : IInsuredCompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdPoolService _idPoolService;
        private readonly ILogger<InsuredCompanyService> _logger;

        public InsuredCompanyService(
            ApplicationDbContext context,
            IIdPoolService idPoolService,
            ILogger<InsuredCompanyService> logger)
        {
            _context = context;
            _idPoolService = idPoolService;
            _logger = logger;
        }

        public async Task<InsuredCompanyDto> SaveInsuredCompanyAsync(InsuredCompanyDto insuredCompanyDto)
        {
            try
            {
                // Verify the company exists
                var companyExists = await _context.Companies.AnyAsync(c => c.Id == insuredCompanyDto.CompanyID);
                if (!companyExists)
                {
                    throw new KeyNotFoundException($"Company with ID {insuredCompanyDto.CompanyID} not found");
                }

                // Check if there's an active insured record for this company
                var existingRecord = await _context.InsuredCompanies
                    .FirstOrDefaultAsync(i => i.CompanyID == insuredCompanyDto.CompanyID && i.ActiveFlag == 1);

                // If creating a new active record, deactivate any existing active records
                if (insuredCompanyDto.ID == 0 && insuredCompanyDto.ActiveFlag == 1 && existingRecord != null)
                {
                    existingRecord.ActiveFlag = 0;
                    _context.Entry(existingRecord).State = EntityState.Modified;
                }

                // Handle create or update based on Id value
                if (insuredCompanyDto.ID == 0)
                {
                    // Create new insured company record with ID from IdPool
                    int newId = await _idPoolService.GetNextIdAsync(IdPoolType.InsuredCompany);

                    var insuredCompany = new InsuredCompany
                    {
                        ID = newId,
                        CompanyID = insuredCompanyDto.CompanyID,
                        StatusCode = insuredCompanyDto.StatusCode ?? 1,
                        SizeCode = insuredCompanyDto.SizeCode ?? 1,
                        InsuranceEntryDate = insuredCompanyDto.InsuranceEntryDate ?? Utils.MinDateTime,
                        OpeningEffecDate = insuredCompanyDto.OpeningEffecDate ?? DateTime.Today,
                        ClosingEffecDate = insuredCompanyDto.ClosingEffecDate ?? DateTime.MaxValue,
                        OpeningRegDate = insuredCompanyDto.OpeningRegDate ?? DateTime.Today,
                        ClosingRegDate = insuredCompanyDto.ClosingRegDate ?? DateTime.MaxValue,
                        OpeningRef = insuredCompanyDto.OpeningRef ?? 0,
                        ClosingRef = insuredCompanyDto.ClosingRef ?? 0,
                        ActiveFlag = insuredCompanyDto.ActiveFlag ?? 1
                    };

                    _context.InsuredCompanies.Add(insuredCompany);
                    await _context.SaveChangesAsync();

                    // Get the company name for the response
                    var company = await _context.Companies.FindAsync(insuredCompanyDto.CompanyID);

                    insuredCompanyDto.ID = newId;
                    insuredCompanyDto.CompanyName = company.RegistrationName;
                }
                else
                {
                    // Update existing insured company record
                    var insuredCompany = await _context.InsuredCompanies.FindAsync(insuredCompanyDto.ID);
                    if (insuredCompany == null)
                    {
                        throw new KeyNotFoundException($"Insured company record with ID {insuredCompanyDto.ID} not found");
                    }

                    // If updating to active, deactivate any other active records for this company
                    if (insuredCompany.ActiveFlag != 1 && insuredCompanyDto.ActiveFlag == 1)
                    {
                        var otherActiveRecords = await _context.InsuredCompanies
                            .Where(i => i.CompanyID == insuredCompanyDto.CompanyID && i.ActiveFlag == 1 && i.ID != insuredCompanyDto.ID)
                            .ToListAsync();

                        foreach (var record in otherActiveRecords)
                        {
                            record.ActiveFlag = 0;
                            _context.Entry(record).State = EntityState.Modified;
                        }
                    }

                    // Update properties
                    insuredCompany.CompanyID = insuredCompanyDto.CompanyID;
                    insuredCompany.StatusCode = insuredCompanyDto.StatusCode ?? 1;
                    insuredCompany.SizeCode = insuredCompanyDto.SizeCode ?? 1;
                    insuredCompany.InsuranceEntryDate = insuredCompanyDto.InsuranceEntryDate;
                    insuredCompany.OpeningEffecDate = insuredCompanyDto.OpeningEffecDate;
                    insuredCompany.ClosingEffecDate = insuredCompanyDto.ClosingEffecDate;
                    insuredCompany.OpeningRegDate = insuredCompanyDto.OpeningRegDate;
                    insuredCompany.ClosingRegDate = insuredCompanyDto.ClosingRegDate;
                    insuredCompany.OpeningRef = insuredCompanyDto.OpeningRef;
                    insuredCompany.ClosingRef = insuredCompanyDto.ClosingRef;
                    insuredCompany.ActiveFlag = insuredCompanyDto.ActiveFlag ?? 1;

                    _context.Entry(insuredCompany).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    // Get the company name for the response
                    var company = await _context.Companies.FindAsync(insuredCompanyDto.CompanyID);
                    insuredCompanyDto.CompanyName = company.RegistrationName;
                }

                return insuredCompanyDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving insured company");
                throw;
            }
        }

        public async Task DeleteInsuredCompanyAsync(int id)
        {
            try
            {
                var insuredCompany = await _context.InsuredCompanies.FindAsync(id);
                if (insuredCompany == null)
                {
                    throw new KeyNotFoundException($"Insured company record with ID {id} not found");
                }

                _context.InsuredCompanies.Remove(insuredCompany);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting insured company with id {Id}", id);
                throw;
            }
        }

        public async Task<bool> InsuredCompanyExistsAsync(int id)
        {
            return await _context.InsuredCompanies.AnyAsync(e => e.ID == id);
        }
    }
}
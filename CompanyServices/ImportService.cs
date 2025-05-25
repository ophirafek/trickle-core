// CompanyServices/ImportService.cs - Updated for new lead structure
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base;
using CompanyServices;
using ACIA.Data;
using ACIA.DTOs;
using ACIA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace ACIA.Services
{
    public class ImportService : IImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompanyService _companyService;
        private readonly ILeadService _leadService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<ImportService> _logger;

        public ImportService(
            ApplicationDbContext context,
            ICompanyService companyService,
            ILeadService leadService,
            IStringLocalizer<SharedResource> localizer,
            ILogger<ImportService> logger)
        {
            _context = context;
            _companyService = companyService;
            _leadService = leadService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IEnumerable<ImportResult>> ImportCompaniesAsync(IEnumerable<CompanyDto> companies)
        {
            var results = new List<ImportResult>();

            foreach (var companyDto in companies)
            {
                try
                {
                    var result = new ImportResult
                    {
                        CompanyName = companyDto.RegistrationName
                    };

                    // Check if company exists by registration number
                    var existingCompany = await _context.Companies
                        .FirstOrDefaultAsync(c => c.RegistrationNumber == companyDto.RegistrationNumber);

                    bool companyExists = existingCompany != null;
                    int companyId = 0;

                    if (!companyExists)
                    {
                        // Create new company using existing service
                        var newCompanyDto = await _companyService.CreateCompanyAsync(companyDto);
                        companyId = newCompanyDto.Id;

                        // Create lead using new structure
                        var leadCreateDto = new LeadCreateDto
                        {
                            LeadName = companyDto.RegistrationName, // Use LeadName instead of Title
                            CompanyId = companyId,
                            StatusCode = "New",
                            SalesGapValue = 0,
                            Probability = 0,
                            LeadSourceCode = null, // Could map to a "Import" source code
                            Notes = "Lead created from import",
                            AdditionalInfo = "Imported company lead"
                        };

                        await _leadService.CreateLeadAsync(leadCreateDto);

                        result.Status = 0; // New company created
                        result.Description = _localizer["CompanyCreatedSuccessfully"];
                    }
                    else
                    {
                        companyId = existingCompany.Id;

                        // Check if active lead exists for this company
                        var existingLeads = await _leadService.GetLeadsByCompanyAsync(companyId);

                        bool leadExists = existingLeads.Any();

                        if (!leadExists)
                        {
                            // Create a new lead for existing company using new structure
                            var leadCreateDto = new LeadCreateDto
                            {
                                LeadName = companyDto.RegistrationName, // Use LeadName instead of Title
                                CompanyId = companyId,
                                StatusCode = "New",
                                SalesGapValue = 0,
                                Probability = 0,
                                LeadSourceCode = null, // Could map to a "Import" source code
                                Notes = "Lead created from import for existing company",
                                AdditionalInfo = "Imported lead for existing company"
                            };

                            await _leadService.CreateLeadAsync(leadCreateDto);

                            result.Status = 1; // Company exists + new lead
                            result.Description = _localizer["ExistingCompanyLeadCreated"];
                        }
                        else
                        {
                            result.Status = 2; // Both exist
                            result.Description = _localizer["CompanyAndLeadExist"];
                        }
                    }

                    results.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error importing company {CompanyName}", companyDto.RegistrationName);

                    results.Add(new ImportResult
                    {
                        Status = 3, // Error
                        CompanyName = companyDto.RegistrationName,
                        ErrorMessage = ex.Message
                    });
                }
            }

            return results;
        }
    }
}
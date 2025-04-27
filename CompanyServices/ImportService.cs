// CompanyServices/ImportService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyServices;
using LeadManagerPro.Data;
using LeadManagerPro.DTOs;
using LeadManagerPro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeadManagerPro.Services
{
    public class ImportService : IImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompanyService _companyService;
        private readonly ILeadService _leadService;
        private readonly ILogger<ImportService> _logger;

        public ImportService(
            ApplicationDbContext context,
            ICompanyService companyService,
            ILeadService leadService,
            ILogger<ImportService> logger)
        {
            _context = context;
            _companyService = companyService;
            _leadService = leadService;
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
                        CompanyName = companyDto.Name
                    };

                    // Check if company exists by registration number
                    // Assuming registration number is stored in a field; adjust as needed
                    var existingCompany = await _context.Companies
                        .FirstOrDefaultAsync(c => c.RegistrationNumber == companyDto.RegistrationNumber);

                    bool companyExists = existingCompany != null;
                    int companyId = 0;

                    if (!companyExists)
                    {
                        // Create new company using existing service
                        var newCompanyDto = await _companyService.CreateCompanyAsync(companyDto);
                        companyId = newCompanyDto.Id;

                        var leadCreateDto = new LeadCreateDto
                        {
                            Title = companyDto.Name,
                            CompanyId = companyId,
                            Value = 0,
                            Probability = 0,
                            Source = "Import",
                            Description = ""
                        };

                        await _leadService.CreateLeadAsync(leadCreateDto);

                        result.Status = 0; // New company created
                        result.Description = "Company created successfully";
                    }
                    else
                    {
                        companyId = existingCompany.Id;
                        
                        // Check if lead exists for this company
                        var existingLeads = await _leadService.GetLeadsByCompanyAsync(companyId);

                        bool leadExists = existingLeads.Any();

                        if (!leadExists)
                        {
                            // Create a new lead for existing company
                            var leadCreateDto = new LeadCreateDto
                            {
                                Title = companyDto.Name,
                                CompanyId = companyId,
                                Value =  0,
                                Probability = 0,
                                Source = "Import",
                                Owner = "",
                                Description = ""
                            };

                            await _leadService.CreateLeadAsync(leadCreateDto);

                            result.Status = 1; // Company exists + new lead
                            result.Description = "Existing company, new lead created";
                        }
                        else
                        {
                            result.Status = 2; // Both exist
                            result.Description = "Company and lead already exist";
                        }
                    }

                    results.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error importing company {CompanyName}", companyDto.Name);

                    results.Add(new ImportResult
                    {
                        Status = 3, // Error
                        CompanyName = companyDto.Name,
                        ErrorMessage = ex.Message
                    });
                }
            }

            return results;
        }
    }
}
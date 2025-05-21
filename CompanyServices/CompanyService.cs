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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ACIA.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyService> _logger;
        private IIdPoolService _poolService;

        public CompanyService(ApplicationDbContext context, ILogger<CompanyService> logger, IIdPoolService poolService)
        {
            _context = context;
            _logger = logger;
            _poolService = poolService;
        }

        public async Task<IEnumerable<CompanyDto>> GetCompaniesAsync()
        {
            try
            {
                var companies = await (
                 from company in _context.Companies
                     .Include(c => c.Contacts)
                     .Include(c => c.Notes)
                 join insured in _context.InsuredCompanies
                     on company.Id equals (int)insured.CompanyID into insuredJoin
                 from insuredCompany in insuredJoin.DefaultIfEmpty()
                 where company.IsActive &&
                       company.OpeningEffectiveDate <= DateTime.UtcNow.Date && company.ClosingEffectiveDate > DateTime.UtcNow.Date &&
                       (insuredCompany == null || insuredCompany.ActiveFlag == 1)
                 select new
                 {
                     Company = company,
                     InsuredCompany = insuredCompany
                 })
                 .ToListAsync();

                var companiesDto = companies
                    .Select(result =>
                    {

                        var c = result.Company;
                        var insuredCompany = result.InsuredCompany;

                        var companyDto = new CompanyDto
                        {
                            Id = c.Id,
                            IdTypeCode = c.IdTypeCode,
                            RegistrationNumber = c.RegistrationNumber,
                            DunsNumber = c.DunsNumber,
                            VATNumber = c.VATNumber,
                            RegistrationName = c.RegistrationName,
                            TradeName = c.TradeName,
                            EnglishName = c.EnglishName,
                            CompanyStatusCode = c.CompanyStatusCode,
                            BusinessFieldCode = c.BusinessFieldCode,
                            EntityTypeCode = c.EntityTypeCode,
                            FoundingYear = c.FoundingYear,
                            CountryCode = c.CountryCode,
                            Website = c.Website,
                            StreetAddress = c.StreetAddress,
                            City = c.City,
                            StateProvince = c.StateProvince,
                            PostalCode = c.PostalCode,
                            PhoneNumber = c.PhoneNumber,
                            MobileNumber = c.MobileNumber,
                            FaxNumber = c.FaxNumber,
                            EMailAddress = c.EMailAddress,
                            Remarks = c.Remarks,
                            LastReportDate = c.LastReportDate,
                            LastReportName = c.LastReportName,
                            OpeningEffectiveDate = c.OpeningEffectiveDate,
                            ClosingEffectiveDate = c.ClosingEffectiveDate,
                            OpeningRegDate = c.OpeningRegDate,
                            ClosingRegDate = c.ClosingRegDate,
                            OpeningRef = c.OpeningRef,
                            ClosingRef = c.ClosingRef,
                            IsInsured = insuredCompany != null,

                    
                            Notes = c.Notes?.Select(n => new NoteDto
                            {
                                Id = n.Id,
                                Title = n.Title,
                                Content = n.Content,
                                CreatedAt = n.CreatedAt,
                                CompanyId = n.CompanyId
                            }).ToList()
                        };
                        if (insuredCompany != null)
                        {
                            companyDto.InsuredDetails = new InsuredCompanyDto
                            {
                                ID = insuredCompany.ID,
                                CompanyID = insuredCompany.CompanyID,
                                CompanyName = c.RegistrationName,
                                StatusCode = insuredCompany.StatusCode,
                                SizeCode = insuredCompany.SizeCode,
                                InsuranceEntryDate = insuredCompany.InsuranceEntryDate,
                                OpeningEffecDate = insuredCompany.OpeningEffecDate,
                                ClosingEffecDate = insuredCompany.ClosingEffecDate,
                                OpeningRegDate = insuredCompany.OpeningRegDate,
                                ClosingRegDate = insuredCompany.ClosingRegDate,
                                OpeningRef = insuredCompany.OpeningRef,
                                ClosingRef = insuredCompany.ClosingRef,
                                ActiveFlag = insuredCompany.ActiveFlag
                            };
                        }
                        return companyDto; }).ToList();
                return companiesDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting companies");
                throw;
            }
        }

        public async Task<CompanyDto> GetCompanyByIdAsync(int id)
        {
            try
            { 
                 var result = await (
                 from company in _context.Companies
                     .Include(c => c.Contacts)
                     .Include(c => c.Notes)
                 join insured in _context.InsuredCompanies
                     on company.Id equals (int)insured.CompanyID into insuredJoin
                 from insuredCompany in insuredJoin.DefaultIfEmpty()
                 where company.Id == id &&
                       company.IsActive &&
                       company.OpeningEffectiveDate <= DateTime.UtcNow.Date && company.ClosingEffectiveDate > DateTime.UtcNow.Date &&
                       (insuredCompany == null || insuredCompany.ActiveFlag == 1)
                 select new
                 {
                     Company = company,
                     InsuredCompany = insuredCompany
                 })
                 .FirstOrDefaultAsync();

                if (result == null)
                {
                    return null;
                }
                var c = result.Company;
                var insuredCompanyResult = result.InsuredCompany;
                var companyDto = new CompanyDto
                {
                    Id = c.Id,
                    IdTypeCode = c.IdTypeCode,
                    RegistrationNumber = c.RegistrationNumber,
                    DunsNumber = c.DunsNumber,
                    VATNumber = c.VATNumber,
                    RegistrationName = c.RegistrationName,
                    TradeName = c.TradeName,
                    EnglishName = c.EnglishName,
                    CompanyStatusCode = c.CompanyStatusCode,
                    BusinessFieldCode = c.BusinessFieldCode,
                    EntityTypeCode = c.EntityTypeCode,
                    FoundingYear = c.FoundingYear,
                    CountryCode = c.CountryCode,
                    Website = c.Website,
                    StreetAddress = c.StreetAddress,
                    City = c.City,
                    StateProvince = c.StateProvince,
                    PostalCode = c.PostalCode,
                    PhoneNumber = c.PhoneNumber,
                    MobileNumber = c.MobileNumber,
                    FaxNumber = c.FaxNumber,
                    EMailAddress = c.EMailAddress,
                    Remarks = c.Remarks,
                    LastReportDate = c.LastReportDate,
                    LastReportName = c.LastReportName,
                    OpeningEffectiveDate = c.OpeningEffectiveDate,
                    ClosingEffectiveDate = c.ClosingEffectiveDate,
                    OpeningRegDate = c.OpeningRegDate,
                    ClosingRegDate = c.ClosingRegDate,
                    OpeningRef = c.OpeningRef,
                    ClosingRef = c.ClosingRef,
                    IsInsured = insuredCompanyResult != null,

                    // Map related collections
                 

                    Notes = c.Notes?.Select(n => new NoteDto
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Content = n.Content,
                        CreatedAt = n.CreatedAt,
                        CompanyId = n.CompanyId
                    }).ToList()
                };
                if (insuredCompanyResult != null)
                {
                    companyDto.InsuredDetails = new InsuredCompanyDto
                    {
                        ID = insuredCompanyResult.ID,
                        CompanyID = insuredCompanyResult.CompanyID,
                        CompanyName = c.RegistrationName,
                        StatusCode = insuredCompanyResult.StatusCode,
                        SizeCode = insuredCompanyResult.SizeCode,
                        InsuranceEntryDate = insuredCompanyResult.InsuranceEntryDate,
                        OpeningEffecDate = insuredCompanyResult.OpeningEffecDate,
                        ClosingEffecDate = insuredCompanyResult.ClosingEffecDate,
                        OpeningRegDate = insuredCompanyResult.OpeningRegDate,
                        ClosingRegDate = insuredCompanyResult.ClosingRegDate,
                        OpeningRef = insuredCompanyResult.OpeningRef,
                        ClosingRef = insuredCompanyResult.ClosingRef,
                        ActiveFlag = insuredCompanyResult.ActiveFlag
                    };
                }
                return companyDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting company with id {Id}", id);
                throw;
            }
        }
        // In CompanyService.cs, update CreateCompanyAsync method:
        public async Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto)
        {
            try
            {
                var company = new Company
                {
                    // ID is auto-generated
                    Id = await _poolService.GetNextIdAsync(IIdPoolService.IdPoolType.Company),
                    RegistrationNumber = companyDto.RegistrationNumber, // This is required
                    IdTypeCode = companyDto.IdTypeCode ?? 1,
                    DunsNumber = companyDto.DunsNumber ?? string.Empty,
                    VATNumber = companyDto.VATNumber ?? string.Empty,
                    RegistrationName = companyDto.RegistrationName, // This is required
                    TradeName = companyDto.TradeName ?? string.Empty,
                    EnglishName = companyDto.EnglishName ?? string.Empty,
                    CompanyStatusCode = companyDto.CompanyStatusCode ?? 0,
                    BusinessFieldCode = companyDto.BusinessFieldCode ?? 0,
                    EntityTypeCode = companyDto.EntityTypeCode ?? 0,
                    FoundingYear = companyDto.FoundingYear ?? 0,
                    CountryCode = companyDto.CountryCode ?? 0,
                    Website = companyDto.Website ?? string.Empty,
                    StreetAddress = companyDto.StreetAddress ?? string.Empty,
                    City = companyDto.City ?? string.Empty,
                    StateProvince = companyDto.StateProvince ?? string.Empty,
                    PostalCode = companyDto.PostalCode ?? string.Empty,
                    PhoneNumber = companyDto.PhoneNumber ?? string.Empty,
                    MobileNumber = companyDto.MobileNumber ?? string.Empty,
                    FaxNumber = companyDto.FaxNumber ?? string.Empty,
                    EMailAddress = companyDto.EMailAddress ?? string.Empty,
                    Remarks = companyDto.Remarks ?? string.Empty,
                    LastReportDate = companyDto.LastReportDate,
                    LastReportName = companyDto.LastReportName ?? string.Empty,
                    OpeningEffectiveDate = companyDto.OpeningEffectiveDate ?? DateTime.Now.Date,
                    ClosingEffectiveDate = companyDto.ClosingEffectiveDate ?? DateTime.MaxValue,
                    OpeningRegDate = companyDto.OpeningRegDate ?? DateTime.UtcNow,
                    ClosingRegDate = companyDto.ClosingRegDate ?? DateTime.MaxValue,
                    OpeningRef = companyDto.OpeningRef ?? string.Empty,
                    ClosingRef = companyDto.ClosingRef ?? string.Empty,
                    IsActive = true
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                // Similar mapping for return DTO
                var newCompanyDto = new CompanyDto
                {
                    Id = company.Id,
                    IdTypeCode = companyDto.IdTypeCode ?? 1,
                    RegistrationNumber = company.RegistrationNumber,
                    DunsNumber = company.DunsNumber,
                    VATNumber = company.VATNumber,
                    RegistrationName = company.RegistrationName,
                    TradeName = company.TradeName,
                    EnglishName = company.EnglishName,
                    CompanyStatusCode = company.CompanyStatusCode,
                    BusinessFieldCode = company.BusinessFieldCode,
                    EntityTypeCode = company.EntityTypeCode,
                    FoundingYear = company.FoundingYear,
                    CountryCode = company.CountryCode,
                    Website = company.Website,
                    StreetAddress = company.StreetAddress,
                    City = company.City,
                    StateProvince = company.StateProvince,
                    PostalCode = company.PostalCode,
                    PhoneNumber = company.PhoneNumber,
                    MobileNumber = company.MobileNumber,
                    FaxNumber = company.FaxNumber,
                    EMailAddress = company.EMailAddress,
                    Remarks = company.Remarks,
                    LastReportDate = company.LastReportDate,
                    LastReportName = company.LastReportName,
                    OpeningEffectiveDate = company.OpeningEffectiveDate,
                    ClosingEffectiveDate = company.ClosingEffectiveDate,
                    OpeningRegDate = company.OpeningRegDate,
                    ClosingRegDate = company.ClosingRegDate,
                    OpeningRef = company.OpeningRef,
                    ClosingRef = company.ClosingRef,
                    IsActive = company.IsActive
                };

                return newCompanyDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating company");
                throw;
            }
        }
        public async Task UpdateCompanyAsync(int id, CompanyDto companyDto)
        {
            try
            {
                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    throw new KeyNotFoundException($"Company with ID {id} not found");
                }

                // Update company properties
                company.RegistrationNumber = companyDto.RegistrationNumber;
                company.DunsNumber = companyDto.DunsNumber;
                company.VATNumber = companyDto.VATNumber;
                company.RegistrationName = companyDto.RegistrationName;
                company.TradeName = companyDto.TradeName;
                company.EnglishName = companyDto.EnglishName;
                company.CompanyStatusCode = companyDto.CompanyStatusCode ?? 0;
                company.BusinessFieldCode = companyDto.BusinessFieldCode ?? 0;
                company.EntityTypeCode = companyDto.EntityTypeCode ?? 0;
                company.FoundingYear = companyDto.FoundingYear ?? 0;
                company.CountryCode = companyDto.CountryCode ?? 0;
                company.Website = companyDto.Website;
                company.StreetAddress = companyDto.StreetAddress;
                company.City = companyDto.City;
                company.StateProvince = companyDto.StateProvince;
                company.PostalCode = companyDto.PostalCode;
                company.PhoneNumber = companyDto.PhoneNumber;
                company.MobileNumber = companyDto.MobileNumber;
                company.FaxNumber = companyDto.FaxNumber;
                company.EMailAddress = companyDto.EMailAddress;
                company.Remarks = companyDto.Remarks;
                company.LastReportDate = companyDto.LastReportDate;
                company.LastReportName = companyDto.LastReportName;
                company.OpeningEffectiveDate = companyDto.OpeningEffectiveDate;
                company.ClosingEffectiveDate = companyDto.ClosingEffectiveDate;
                company.OpeningRegDate = companyDto.OpeningRegDate;
                company.ClosingRegDate = companyDto.ClosingRegDate;
                company.OpeningRef = companyDto.OpeningRef;
                company.ClosingRef = companyDto.ClosingRef;

                _context.Entry(company).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating company with id {Id}", id);
                throw;
            }
        }
        public async Task DeleteCompanyAsync(int id)
        {
            try
            {
                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    throw new KeyNotFoundException($"Company with ID {id} not found");
                }

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting company with id {Id}", id);
                throw;
            }
        }

        public async Task<bool> CompanyExistsAsync(int id)
        {
            return await _context.Companies.AnyAsync(e => e.Id == id);
        }

     
        public async Task<NoteDto> AddNoteAsync(int companyId, NoteCreateDto noteDto)
        {
            try
            {
                // Ensure the company exists
                var companyExists = await _context.Companies.AnyAsync(c => c.Id == companyId);
                if (!companyExists)
                {
                    throw new KeyNotFoundException($"Company with ID {companyId} not found");
                }

                // Ensure the company ID in the DTO matches the route parameter
                if (noteDto.CompanyId != companyId)
                {
                    noteDto.CompanyId = companyId;
                }

                var note = new Note
                {
                    Title = noteDto.Title,
                    Content = noteDto.Content,
                    CompanyId = noteDto.CompanyId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notes.Add(note);
                await _context.SaveChangesAsync();

                var newNoteDto = new NoteDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    Content = note.Content,
                    CompanyId = note.CompanyId,
                    CreatedAt = note.CreatedAt
                };

                return newNoteDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding note to company with id {CompanyId}", companyId);
                throw;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeadManagerPro.Data;
using LeadManagerPro.DTOs;
using LeadManagerPro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeadManagerPro.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(ApplicationDbContext context, ILogger<CompanyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CompanyDto>> GetCompaniesAsync()
        {
            try
            {
                var companies = await _context.Companies
                    .Select(c => new CompanyDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Industry = c.Industry,
                        Size = c.Size,
                        Location = c.Location,
                        Website = c.Website,
                        Status = c.Status,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToListAsync();

                return companies;
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
                var company = await _context.Companies
                    .Include(c => c.Contacts)
                    .Include(c => c.Notes)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (company == null)
                {
                    return null;
                }

                var companyDto = new CompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Industry = company.Industry,
                    Size = company.Size,
                    Location = company.Location,
                    Website = company.Website,
                    Status = company.Status,
                    StreetAddress = company.StreetAddress,
                    Suite = company.Suite,
                    City = company.City,
                    StateProvince = company.StateProvince,
                    PostalCode = company.PostalCode,
                    Country = company.Country,
                    BillingStreet = company.BillingStreet,
                    BillingCity = company.BillingCity,
                    BillingPostalCode = company.BillingPostalCode,
                    LinkedInProfile = company.LinkedInProfile,
                    FoundingYear = company.FoundingYear,
                    Description = company.Description,
                    RegistrationNumber = company.RegistrationNumber,
                    DunsNumber = company.DunsNumber,
                    CreatedAt = company.CreatedAt,
                    UpdatedAt = company.UpdatedAt,
                    Contacts = company.Contacts?.Select(c => new ContactDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        JobTitle = c.JobTitle,
                        Email = c.Email,
                        Phone = c.Phone,
                        CompanyId = c.CompanyId
                    }).ToList(),
                    Notes = company.Notes?.Select(n => new NoteDto
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Content = n.Content,
                        CreatedAt = n.CreatedAt,
                        CompanyId = n.CompanyId
                    }).ToList()
                };

                return companyDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting company with id {Id}", id);
                throw;
            }
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto)
        {
            try
            {
                var company = new Company
                {
                    // ID is auto-generated, so we don't set it here
                    Name = companyDto.Name, // This is required
                    Industry = companyDto.Industry ?? string.Empty,
                    Size = companyDto.Size ?? string.Empty,
                    Location = companyDto.Location ?? string.Empty,
                    Website = companyDto.Website ?? string.Empty,
                    Status = companyDto.Status ?? "Active",
                    StreetAddress = companyDto.StreetAddress ?? string.Empty,
                    Suite = companyDto.Suite ?? string.Empty,
                    City = companyDto.City ?? string.Empty, // Handle null City
                    StateProvince = companyDto.StateProvince ?? string.Empty,
                    PostalCode = companyDto.PostalCode ?? string.Empty,
                    Country = companyDto.Country ?? string.Empty,
                    BillingStreet = companyDto.BillingStreet ?? string.Empty,
                    BillingCity = companyDto.BillingCity ?? string.Empty,
                    BillingPostalCode = companyDto.BillingPostalCode ?? string.Empty,
                    LinkedInProfile = companyDto.LinkedInProfile ?? string.Empty,
                    FoundingYear = companyDto.FoundingYear,
                    Description = companyDto.Description ?? string.Empty,
                    RegistrationNumber = companyDto.RegistrationNumber ?? string.Empty,  // Added field
                    DunsNumber = companyDto.DunsNumber ?? string.Empty,                 // Added field

                    CreatedAt = DateTime.UtcNow
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                var newCompanyDto = new CompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Industry = company.Industry,
                    Size = company.Size,
                    Location = company.Location,
                    Website = company.Website,
                    Status = company.Status,
                    StreetAddress = company.StreetAddress,
                    Suite = company.Suite,
                    City = company.City,
                    StateProvince = company.StateProvince,
                    PostalCode = company.PostalCode,
                    Country = company.Country,
                    BillingStreet = company.BillingStreet,
                    BillingCity = company.BillingCity,
                    BillingPostalCode = company.BillingPostalCode,
                    LinkedInProfile = company.LinkedInProfile,
                    FoundingYear = company.FoundingYear,
                    Description = company.Description,
                    RegistrationNumber = company.RegistrationNumber,  // Added field
                    DunsNumber = company.DunsNumber,                 // Added field
                    CreatedAt = company.CreatedAt,
                    UpdatedAt = company.UpdatedAt
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
                company.Name = companyDto.Name;
                company.Industry = companyDto.Industry;
                company.Size = companyDto.Size;
                company.Location = companyDto.Location;
                company.Website = companyDto.Website;
                company.Status = companyDto.Status;
                company.StreetAddress = companyDto.StreetAddress;
                company.Suite = companyDto.Suite;
                company.City = companyDto.City;
                company.StateProvince = companyDto.StateProvince;
                company.PostalCode = companyDto.PostalCode;
                company.Country = companyDto.Country;
                company.BillingStreet = companyDto.BillingStreet;
                company.BillingCity = companyDto.BillingCity;
                company.BillingPostalCode = companyDto.BillingPostalCode;
                company.LinkedInProfile = companyDto.LinkedInProfile;
                company.FoundingYear = companyDto.FoundingYear;
                company.Description = companyDto.Description;
                company.RegistrationNumber = companyDto.RegistrationNumber;  // Added field
                company.DunsNumber = companyDto.DunsNumber;                 // Added field

                company.UpdatedAt = DateTime.UtcNow;

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

        public async Task<ContactDto> AddContactAsync(int companyId, ContactCreateDto contactDto)
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
                if (contactDto.CompanyId != companyId)
                {
                    contactDto.CompanyId = companyId;
                }

                var contact = new Contact
                {
                    Name = contactDto.Name,
                    JobTitle = contactDto.JobTitle,
                    Email = contactDto.Email,
                    Phone = contactDto.Phone,
                    CompanyId = contactDto.CompanyId
                };

                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                var newContactDto = new ContactDto
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    JobTitle = contact.JobTitle,
                    Email = contact.Email,
                    Phone = contact.Phone,
                    CompanyId = contact.CompanyId
                };

                return newContactDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding contact to company with id {CompanyId}", companyId);
                throw;
            }
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
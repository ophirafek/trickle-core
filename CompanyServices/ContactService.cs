using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACIA.Data;
using ACIA.DTOs;
using ACIA.Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Interfaces.IIdPoolService;

namespace ACIA.Services
{
    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdPoolService _idPoolService;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            ApplicationDbContext context,
            IIdPoolService idPoolService,
            ILogger<ContactService> logger)
        {
            _context = context;
            _idPoolService = idPoolService;
            _logger = logger;
        }

        public async Task<ContactDto> GetContactByIdAsync(int id)
        {
            try
            {
                var contact = await _context.Contacts
                    .Include(c => c.Company)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (contact == null)
                {
                    return null;
                }

                var contactDto = new ContactDto
                {
                    Id = contact.Id,
                    CompanyId = contact.CompanyId,
                    CompanyName = contact.Company?.RegistrationName,
                    ContactCode = contact.ContactCode,
                    PersonalId = contact.PersonalId,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Name = contact.Name,
                    PhoneNumber = contact.PhoneNumber,
                    CellNumber = contact.CellNumber,
                    FaxNumber = contact.FaxNumber,
                    Email = contact.Email,
                    Position = contact.Position,
                    Department = contact.Department,
                    Notes = contact.Notes,
                    OpeningEffecDate = contact.OpeningEffecDate,
                    ClosingEffecDate = contact.ClosingEffecDate,
                    OpeningRegDate = contact.OpeningRegDate,
                    ClosingRegDate = contact.ClosingRegDate,
                    OpeningRef = contact.OpeningRef,
                    ClosingRef = contact.ClosingRef,
                    ActiveFlag = contact.ActiveFlag
                };

                return contactDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting contact with id {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ContactDto>> GetContactsByCompanyAsync(int companyId)
        {
            try
            {
                var contacts = await _context.Contacts
                    .Include(c => c.Company)
                    .Where(c => c.CompanyId == companyId && c.ActiveFlag)
                    .Select(c => new ContactDto
                    {
                        Id = c.Id,
                        CompanyId = c.CompanyId,
                        CompanyName = c.Company.RegistrationName,
                        ContactCode = c.ContactCode,
                        PersonalId = c.PersonalId,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Name = c.Name,
                        PhoneNumber = c.PhoneNumber,
                        CellNumber = c.CellNumber,
                        FaxNumber = c.FaxNumber,
                        Email = c.Email,
                        Position = c.Position,
                        Department = c.Department,
                        Notes = c.Notes,
                        OpeningEffecDate = c.OpeningEffecDate,
                        ClosingEffecDate = c.ClosingEffecDate,
                        OpeningRegDate = c.OpeningRegDate,
                        ClosingRegDate = c.ClosingRegDate,
                        OpeningRef = c.OpeningRef,
                        ClosingRef = c.ClosingRef,
                        ActiveFlag = c.ActiveFlag
                    })
                    .ToListAsync();

                return contacts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting contacts for company {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<ContactDto> SaveContactAsync(ContactDto contactDto)
        {
            try
            {
                // Verify the company exists
                var companyExists = await _context.Companies.AnyAsync(c => c.Id == contactDto.CompanyId);
                if (!companyExists)
                {
                    throw new KeyNotFoundException($"Company with ID {contactDto.CompanyId} not found");
                }

                if (contactDto.Id == 0)
                {
                    // This is a new contact
                    return await CreateContactAsync(contactDto);
                }
                else
                {
                    // This is an update to an existing contact
                    return await UpdateContactAsync(contactDto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving contact");
                throw;
            }
        }

        private async Task<ContactDto> CreateContactAsync(ContactDto contactDto)
        {
            // Get the next ID from the ID pool
            int newId = await _idPoolService.GetNextIdAsync(IdPoolType.Contact);

            var contact = new Contact
            {
                Id = newId,
                CompanyId = contactDto.CompanyId,
                ContactCode = contactDto.ContactCode,
                PersonalId = contactDto.PersonalId,
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                PhoneNumber = contactDto.PhoneNumber,
                CellNumber = contactDto.CellNumber,
                FaxNumber = contactDto.FaxNumber,
                Email = contactDto.Email,
                Position = contactDto.Position,
                Department = contactDto.Department,
                Notes = contactDto.Notes,
                OpeningEffecDate = contactDto.OpeningEffecDate ?? DateTime.UtcNow.Date,
                ClosingEffecDate = contactDto.ClosingEffecDate ?? DateTime.MaxValue,
                OpeningRegDate = DateTime.UtcNow,
                ClosingRegDate = DateTime.MaxValue,
                OpeningRef = contactDto.OpeningRef ?? 0, // Could be the user ID who created the record
                ClosingRef = null,
                ActiveFlag = true
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            // Get the company name for the response
            var company = await _context.Companies.FindAsync(contactDto.CompanyId);

            var newContactDto = new ContactDto
            {
                Id = contact.Id,
                CompanyId = contact.CompanyId,
                CompanyName = company?.RegistrationName,
                ContactCode = contact.ContactCode,
                PersonalId = contact.PersonalId,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Name = contact.Name,
                PhoneNumber = contact.PhoneNumber,
                CellNumber = contact.CellNumber,
                FaxNumber = contact.FaxNumber,
                Email = contact.Email,
                Position = contact.Position,
                Department = contact.Department,
                Notes = contact.Notes,
                OpeningEffecDate = contact.OpeningEffecDate,
                ClosingEffecDate = contact.ClosingEffecDate,
                OpeningRegDate = contact.OpeningRegDate,
                ClosingRegDate = contact.ClosingRegDate,
                OpeningRef = contact.OpeningRef,
                ClosingRef = contact.ClosingRef,
                ActiveFlag = contact.ActiveFlag
            };

            return newContactDto;
        }

        private async Task<ContactDto> UpdateContactAsync(ContactDto contactDto)
        {
            // Find the existing contact
            var contact = await _context.Contacts
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == contactDto.Id && c.ActiveFlag);

            if (contact == null)
            {
                throw new KeyNotFoundException($"Active contact with ID {contactDto.Id} not found");
            }

            // Verify the company exists if different from the current one
            if (contact.CompanyId != contactDto.CompanyId)
            {
                var companyExists = await _context.Companies.AnyAsync(c => c.Id == contactDto.CompanyId);
                if (!companyExists)
                {
                    throw new KeyNotFoundException($"Company with ID {contactDto.CompanyId} not found");
                }
            }

            // Deactivate the current contact
            contact.ActiveFlag = false;
            contact.ClosingEffecDate = DateTime.UtcNow.Date;
            contact.ClosingRegDate = DateTime.UtcNow;
            contact.ClosingRef = contactDto.ClosingRef ?? 0; // Could be the user ID who updated the record

            _context.Entry(contact).State = EntityState.Modified;

            // Get the next ID from the ID pool
            int newId = await _idPoolService.GetNextIdAsync(IdPoolType.Contact);

            // Create a new contact record with updated data
            var newContact = new Contact
            {
                Id = newId,
                CompanyId = contactDto.CompanyId,
                ContactCode = contactDto.ContactCode,
                PersonalId = contactDto.PersonalId,
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                PhoneNumber = contactDto.PhoneNumber,
                CellNumber = contactDto.CellNumber,
                FaxNumber = contactDto.FaxNumber,
                Email = contactDto.Email,
                Position = contactDto.Position,
                Department = contactDto.Department,
                Notes = contactDto.Notes,
                OpeningEffecDate = contactDto.OpeningEffecDate ?? DateTime.UtcNow.Date,
                ClosingEffecDate = contactDto.ClosingEffecDate ?? DateTime.MaxValue,
                OpeningRegDate = DateTime.UtcNow,
                ClosingRegDate = DateTime.MaxValue,
                OpeningRef = contact.Id, // Reference to the previous record
                ClosingRef = null,
                ActiveFlag = true
            };

            _context.Contacts.Add(newContact);
            await _context.SaveChangesAsync();

            // Get the company name for the response
            var company = contact.Company;
            if (company == null || company.Id != contactDto.CompanyId)
            {
                company = await _context.Companies.FindAsync(contactDto.CompanyId);
            }

            var updatedContactDto = new ContactDto
            {
                Id = newContact.Id,
                CompanyId = newContact.CompanyId,
                CompanyName = company?.RegistrationName,
                ContactCode = newContact.ContactCode,
                PersonalId = newContact.PersonalId,
                FirstName = newContact.FirstName,
                LastName = newContact.LastName,
                Name = newContact.Name,
                PhoneNumber = newContact.PhoneNumber,
                CellNumber = newContact.CellNumber,
                FaxNumber = newContact.FaxNumber,
                Email = newContact.Email,
                Position = newContact.Position,
                Department = newContact.Department,
                Notes = newContact.Notes,
                OpeningEffecDate = newContact.OpeningEffecDate,
                ClosingEffecDate = newContact.ClosingEffecDate,
                OpeningRegDate = newContact.OpeningRegDate,
                ClosingRegDate = newContact.ClosingRegDate,
                OpeningRef = newContact.OpeningRef,
                ClosingRef = newContact.ClosingRef,
                ActiveFlag = newContact.ActiveFlag
            };

            return updatedContactDto;
        }

        public async Task DeleteContactAsync(int id)
        {
            try
            {
                var contact = await _context.Contacts.FindAsync(id);
                if (contact == null)
                {
                    throw new KeyNotFoundException($"Contact with ID {id} not found");
                }

                // Soft delete - just mark as inactive instead of removing
                contact.ActiveFlag = false;
                contact.ClosingEffecDate = DateTime.UtcNow.Date;
                contact.ClosingRegDate = DateTime.UtcNow;

                _context.Entry(contact).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting contact with id {Id}", id);
                throw;
            }
        }

        public async Task<bool> ContactExistsAsync(int id)
        {
            return await _context.Contacts.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> ActiveContactExistsAsync(int id)
        {
            return await _context.Contacts.AnyAsync(e => e.Id == id && e.ActiveFlag);
        }
    }
}
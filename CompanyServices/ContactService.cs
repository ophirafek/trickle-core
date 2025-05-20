using System;
using System.Threading.Tasks;
using ACIA.Data;
using ACIA.DTOs;
using ACIA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ACIA.Services
{
    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContactService> _logger;

        public ContactService(ApplicationDbContext context, ILogger<ContactService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ContactDto> GetContactByIdAsync(int id)
        {
            try
            {
                var contact = await _context.Contacts.FindAsync(id);

                if (contact == null)
                {
                    return null;
                }

                var contactDto = new ContactDto
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    JobTitle = contact.JobTitle,
                    Email = contact.Email,
                    Phone = contact.Phone,
                    CompanyId = contact.CompanyId
                };

                return contactDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting contact with id {Id}", id);
                throw;
            }
        }

        public async Task UpdateContactAsync(int id, ContactUpdateDto contactDto)
        {
            try
            {
                var contact = await _context.Contacts.FindAsync(id);
                if (contact == null)
                {
                    throw new KeyNotFoundException($"Contact with ID {id} not found");
                }

                // Update contact properties
                contact.Name = contactDto.Name;
                contact.JobTitle = contactDto.JobTitle;
                contact.Email = contactDto.Email;
                contact.Phone = contactDto.Phone;
                // Note: We don't update CompanyId here to prevent moving contacts between companies

                _context.Entry(contact).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating contact with id {Id}", id);
                throw;
            }
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

                _context.Contacts.Remove(contact);
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
    }
}
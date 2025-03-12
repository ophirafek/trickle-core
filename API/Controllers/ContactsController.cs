using System;
using System.Threading.Tasks;
using LeadManagerPro.DTOs;
using LeadManagerPro.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeadManagerPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(IContactService contactService, ILogger<ContactsController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDto>> GetContact(int id)
        {
            try
            {
                var contactDto = await _contactService.GetContactByIdAsync(id);

                if (contactDto == null)
                {
                    return NotFound();
                }

                return contactDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting contact with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/Contacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, ContactUpdateDto contactDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!await _contactService.ContactExistsAsync(id))
                {
                    return NotFound();
                }

                await _contactService.UpdateContactAsync(id, contactDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating contact with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            try
            {
                if (!await _contactService.ContactExistsAsync(id))
                {
                    return NotFound();
                }

                await _contactService.DeleteContactAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting contact with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
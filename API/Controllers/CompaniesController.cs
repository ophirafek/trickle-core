// API/Controllers/CompaniesController.cs - Updated with Lead endpoints
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyServices;
using ACIA.DTOs;
using ACIA.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACIA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IImportService _importService;
        private readonly IContactService _contactService;
        private readonly ILeadService _leadService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(
            ICompanyService companyService,
            IImportService importService,
            IContactService contactService,
            ILeadService leadService,
            ILogger<CompaniesController> logger)
        {
            _companyService = companyService;
            _importService = importService;
            _contactService = contactService;
            _leadService = leadService;
            _logger = logger;
        }

        // GET: api/Companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies()
        {
            try
            {
                var companies = await _companyService.GetCompaniesAsync();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting companies");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> GetCompany(int id)
        {
            try
            {
                var companyDto = await _companyService.GetCompanyByIdAsync(id);
                if (companyDto == null)
                {
                    return NotFound();
                }

                return Ok(companyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting company with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/Companies
        [HttpPost]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CompanyDto companyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newCompanyDto = await _companyService.CreateCompanyAsync(companyDto);
                return CreatedAtAction(nameof(GetCompany), new { id = newCompanyDto.Id }, newCompanyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating company");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/Companies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, CompanyDto companyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!await _companyService.CompanyExistsAsync(id))
                {
                    return NotFound();
                }

                await _companyService.UpdateCompanyAsync(id, companyDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating company with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                if (!await _companyService.CompanyExistsAsync(id))
                {
                    return NotFound();
                }

                await _companyService.DeleteCompanyAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting company with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Companies/{companyId}/contacts
        [HttpGet("{companyId}/contacts")]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetCompanyContacts(int companyId)
        {
            try
            {
                // Verify the company exists
                if (!await _companyService.CompanyExistsAsync(companyId))
                {
                    return NotFound($"Company with ID {companyId} not found");
                }

                var contacts = await _contactService.GetContactsByCompanyAsync(companyId);
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting contacts for company {CompanyId}", companyId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/Companies/{companyId}/contacts
        [HttpPost("{companyId}/contacts")]
        public async Task<ActionResult<ContactDto>> AddCompanyContact(int companyId, ContactDto contactDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Ensure the company exists
                if (!await _companyService.CompanyExistsAsync(companyId))
                {
                    return NotFound($"Company with ID {companyId} not found");
                }

                // Ensure the company ID in the DTO matches the route parameter
                if (contactDto.CompanyId != companyId)
                {
                    contactDto.CompanyId = companyId;
                }

                // Set ID to 0 to ensure it's treated as a new contact
                contactDto.Id = 0;

                var newContact = await _contactService.SaveContactAsync(contactDto);
                return CreatedAtAction(nameof(GetCompanyContacts), new { companyId = companyId }, newContact);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding contact to company with id {CompanyId}", companyId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Companies/{companyId}/leads
        [HttpGet("{companyId}/leads")]
        public async Task<ActionResult<IEnumerable<LeadDto>>> GetCompanyLeads(int companyId)
        {
            try
            {
                // Verify the company exists
                if (!await _companyService.CompanyExistsAsync(companyId))
                {
                    return NotFound($"Company with ID {companyId} not found");
                }

                var leads = await _leadService.GetLeadsByCompanyAsync(companyId);
                return Ok(leads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting leads for company {CompanyId}", companyId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/Companies/{companyId}/leads
        [HttpPost("{companyId}/leads")]
        public async Task<ActionResult<LeadDto>> AddCompanyLead(int companyId, LeadCreateDto leadDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Ensure the company exists
                if (!await _companyService.CompanyExistsAsync(companyId))
                {
                    return NotFound($"Company with ID {companyId} not found");
                }

                // Ensure the company ID in the DTO matches the route parameter
                if (leadDto.CompanyId != companyId)
                {
                    leadDto.CompanyId = companyId;
                }

                // Validate contact ID if provided (must belong to the same company)
                if (leadDto.ContactId.HasValue)
                {
                    var contacts = await _contactService.GetContactsByCompanyAsync(companyId);
                    if (!contacts.Any(c => c.Id == leadDto.ContactId.Value))
                    {
                        return BadRequest($"Contact with ID {leadDto.ContactId} does not belong to company {companyId}");
                    }
                }

                var newLead = await _leadService.CreateLeadAsync(leadDto);
                return CreatedAtAction(nameof(GetCompanyLeads), new { companyId = companyId }, newLead);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding lead to company with id {CompanyId}", companyId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("import")]
        public async Task<ActionResult<IEnumerable<ImportResult>>> ImportCompanies(List<CompanyDto> companies)
        {
            try
            {
                IEnumerable<ImportResult> ret = await _importService.ImportCompaniesAsync(companies);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing companies");
                return StatusCode(500, "An error occurred during import");
            }
        }

        // POST: api/Companies/{companyId}/notes
        [HttpPost("{companyId}/notes")]
        public async Task<ActionResult<NoteDto>> AddNote(int companyId, NoteCreateDto noteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Ensure the company exists
                if (!await _companyService.CompanyExistsAsync(companyId))
                {
                    return NotFound($"Company with ID {companyId} not found");
                }

                // Ensure the company ID in the DTO matches the route parameter
                if (noteDto.CompanyId != companyId)
                {
                    noteDto.CompanyId = companyId;
                }

                var newNoteDto = await _companyService.AddNoteAsync(companyId, noteDto);
                return CreatedAtAction(nameof(GetCompany), new { id = companyId }, newNoteDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding note to company with id {CompanyId}", companyId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
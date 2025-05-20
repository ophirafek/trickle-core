using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACIA.DTOs;
using ACIA.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACIA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController : ControllerBase
    {
        private readonly ILeadService _leadService;
        private readonly ILogger<LeadsController> _logger;

        public LeadsController(ILeadService leadService, ILogger<LeadsController> logger)
        {
            _leadService = leadService;
            _logger = logger;
        }

        // GET: api/Leads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeadDto>>> GetLeads()
        {
            try
            {
                var leads = await _leadService.GetLeadsAsync();
                return Ok(leads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting leads");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Leads/Company/5
        [HttpGet("Company/{companyId}")]
        public async Task<ActionResult<IEnumerable<LeadDto>>> GetLeadsByCompany(int companyId)
        {
            try
            {
                var leads = await _leadService.GetLeadsByCompanyAsync(companyId);
                return Ok(leads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting leads for company {CompanyId}", companyId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Leads/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeadDto>> GetLead(int id)
        {
            try
            {
                var lead = await _leadService.GetLeadByIdAsync(id);
                if (lead == null)
                {
                    return NotFound();
                }

                return Ok(lead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting lead with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/Leads
        [HttpPost]
        public async Task<ActionResult<LeadDto>> CreateLead(LeadCreateDto leadDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newLead = await _leadService.CreateLeadAsync(leadDto);
                return CreatedAtAction(nameof(GetLead), new { id = newLead.Id }, newLead);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating lead");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/Leads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLead(int id, LeadUpdateDto leadDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!await _leadService.LeadExistsAsync(id))
                {
                    return NotFound();
                }

                await _leadService.UpdateLeadAsync(id, leadDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating lead with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/Leads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLead(int id)
        {
            try
            {
                if (!await _leadService.LeadExistsAsync(id))
                {
                    return NotFound();
                }

                await _leadService.DeleteLeadAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting lead with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
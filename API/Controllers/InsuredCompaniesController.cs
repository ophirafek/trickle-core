// API/Controllers/InsuredCompaniesController.cs
using System;
using System.Threading.Tasks;
using ACIA.DTOs;
using ACIA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeadManagerPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsuredCompaniesController : ControllerBase
    {
        private readonly IInsuredCompanyService _insuredCompanyService;
        private readonly ILogger<InsuredCompaniesController> _logger;

        public InsuredCompaniesController(IInsuredCompanyService insuredCompanyService, ILogger<InsuredCompaniesController> logger)
        {
            _insuredCompanyService = insuredCompanyService;
            _logger = logger;
        }

        // POST: api/InsuredCompanies
        [HttpPost]
        public async Task<ActionResult<InsuredCompanyDto>> SaveInsuredCompany(InsuredCompanyDto insuredCompanyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var savedInsuredCompany = await _insuredCompanyService.SaveInsuredCompanyAsync(insuredCompanyDto);

                if (insuredCompanyDto.ID == 0)
                {
                    // This was a create operation
                    return CreatedAtAction(nameof(GetInsuredCompany), new { id = savedInsuredCompany.ID }, savedInsuredCompany);
                }
                else
                {
                    // This was an update operation
                    return Ok(savedInsuredCompany);
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving insured company");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // Helper method for CreatedAtAction result
        [NonAction]
        private ActionResult<InsuredCompanyDto> GetInsuredCompany(int id)
        {
            return null; // Not actually called, just needed for route generation
        }

        // DELETE: api/InsuredCompanies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInsuredCompany(int id)
        {
            try
            {
                if (!await _insuredCompanyService.InsuredCompanyExistsAsync(id))
                {
                    return NotFound();
                }

                await _insuredCompanyService.DeleteInsuredCompanyAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting insured company with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
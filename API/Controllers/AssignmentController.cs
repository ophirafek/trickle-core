// API/Controllers/AssignmentsController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACIA.DTOs;
using ACIA.Services;
using ACIA.DTOs;
using ACIA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACIA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;
        private readonly ILogger<AssignmentsController> _logger;

        public AssignmentsController(IAssignmentService assignmentService, ILogger<AssignmentsController> logger)
        {
            _assignmentService = assignmentService;
            _logger = logger;
        }

        // GET: api/Assignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAssignments()
        {
            try
            {
                var assignments = await _assignmentService.GetAssignmentsAsync();
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assignments");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Assignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssignmentDto>> GetAssignment(int id)
        {
            try
            {
                var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
                if (assignment == null)
                {
                    return NotFound();
                }

                return Ok(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assignment with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Assignments/Company/5
        [HttpGet("Company/{companyId}")]
        public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAssignmentsByCompany(int companyId)
        {
            try
            {
                var assignments = await _assignmentService.GetAssignmentsByCompanyAsync(companyId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assignments for company {CompanyId}", companyId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Assignments/Employee/5
        [HttpGet("Employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAssignmentsByEmployee(int employeeId)
        {
            try
            {
                var assignments = await _assignmentService.GetAssignmentsByEmployeeAsync(employeeId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assignments for employee {EmployeeId}", employeeId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/Assignments
        [HttpPost]
        public async Task<ActionResult<AssignmentDto>> SaveAssignment(AssignmentDto assignmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var savedAssignment = await _assignmentService.SaveAssignmentAsync(assignmentDto);

                if (assignmentDto.Id == 0)
                {
                    // This was a create operation
                    return CreatedAtAction(nameof(GetAssignment), new { id = savedAssignment.Id }, savedAssignment);
                }
                else
                {
                    // This was an update operation
                    return Ok(savedAssignment);
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving assignment");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/Assignments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            try
            {
                if (!await _assignmentService.AssignmentExistsAsync(id))
                {
                    return NotFound();
                }

                await _assignmentService.DeleteAssignmentAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting assignment with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
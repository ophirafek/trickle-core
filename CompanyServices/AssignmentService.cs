// CompanyServices/AssignmentService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACIA.Data;
using ACIA.DTOs;
using ACIA.Models;
using ACIA.Services;
using Interfaces;
using ACIA.Data;
using ACIA.DTOs;
using ACIA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Interfaces.IIdPoolService;

namespace ACIA.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdPoolService _idPoolService;
        private readonly ILogger<AssignmentService> _logger;

        public AssignmentService(
            ApplicationDbContext context,
            IIdPoolService idPoolService,
            ILogger<AssignmentService> logger)
        {
            _context = context;
            _idPoolService = idPoolService;
            _logger = logger;
        }

        public async Task<IEnumerable<AssignmentDto>> GetAssignmentsAsync()
        {
            try
            {
                var assignments = await _context.Assignments
                    .Include(a => a.Company)
                    .Select(a => new AssignmentDto
                    {
                        Id = a.Id,
                        CompanyID = a.CompanyID,
                        CompanyName = a.Company.RegistrationName,    
                        AssignmentTypeCode = a.AssignmentTypeCode,
                        EmployeeID = a.EmployeeID,
                        EmployeeName = "", // Would need to fetch from employee service or join with Employee table
                        OpeningEffecDate = a.OpeningEffecDate,
                        ClosingEffecDate = a.ClosingEffecDate,
                        OpeningRegDate = a.OpeningRegDate,
                        ClosingRegDate = a.ClosingRegDate,
                        OpeningRef = a.OpeningRef,
                        ClosingRef = a.ClosingRef,
                        ActiveFlag = a.ActiveFlag
                    })
                    .ToListAsync();

                return assignments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assignments");
                throw;
            }
        }

        public async Task<IEnumerable<AssignmentDto>> GetAssignmentsByCompanyAsync(int companyId)
        {
            try
            {
                var assignments = await _context.Assignments
                    .Include(a => a.Company)
                    .Where(a => a.CompanyID == companyId)
                    .Select(a => new AssignmentDto
                    {
                        Id = a.Id,
                        CompanyID = a.CompanyID,
                        CompanyName = a.Company.RegistrationName,
                        AssignmentTypeCode = a.AssignmentTypeCode,
                        EmployeeID = a.EmployeeID,
                        EmployeeName = "", // Would need to fetch from employee service
                        OpeningEffecDate = a.OpeningEffecDate,
                        ClosingEffecDate = a.ClosingEffecDate,
                        OpeningRegDate = a.OpeningRegDate,
                        ClosingRegDate = a.ClosingRegDate,
                        OpeningRef = a.OpeningRef,
                        ClosingRef = a.ClosingRef,
                        ActiveFlag = a.ActiveFlag
                    })
                    .ToListAsync();

                return assignments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assignments for company {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<IEnumerable<AssignmentDto>> GetAssignmentsByEmployeeAsync(int employeeId)
        {
            try
            {
                var assignments = await _context.Assignments
                    .Include(a => a.Company)
                    .Where(a => a.EmployeeID == employeeId)
                    .Select(a => new AssignmentDto
                    {
                        Id = a.Id,
                        CompanyID = a.CompanyID,
                        CompanyName = a.Company.RegistrationName,
                        AssignmentTypeCode = a.AssignmentTypeCode,
                        EmployeeID = a.EmployeeID,
                        EmployeeName = "", // Would need to fetch from employee service
                        OpeningEffecDate = a.OpeningEffecDate,
                        ClosingEffecDate = a.ClosingEffecDate,
                        OpeningRegDate = a.OpeningRegDate,
                        ClosingRegDate = a.ClosingRegDate,
                        OpeningRef = a.OpeningRef,
                        ClosingRef = a.ClosingRef,
                        ActiveFlag = a.ActiveFlag
                    })
                    .ToListAsync();

                return assignments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assignments for employee {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<AssignmentDto> GetAssignmentByIdAsync(int id)
        {
            try
            {
                var assignment = await _context.Assignments
                    .Include(a => a.Company)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (assignment == null)
                {
                    return null;
                }

                var assignmentDto = new AssignmentDto
                {
                    Id = assignment.Id,
                    CompanyID = assignment.CompanyID,
                    CompanyName = assignment.Company.RegistrationName,
                    AssignmentTypeCode = assignment.AssignmentTypeCode,
                    EmployeeID = assignment.EmployeeID,
                    EmployeeName = "", // Would need to fetch from employee service
                    OpeningEffecDate = assignment.OpeningEffecDate,
                    ClosingEffecDate = assignment.ClosingEffecDate,
                    OpeningRegDate = assignment.OpeningRegDate,
                    ClosingRegDate = assignment.ClosingRegDate,
                    OpeningRef = assignment.OpeningRef,
                    ClosingRef = assignment.ClosingRef,
                    ActiveFlag = assignment.ActiveFlag
                };

                return assignmentDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assignment with id {Id}", id);
                throw;
            }
        }

        public async Task<AssignmentDto> SaveAssignmentAsync(AssignmentDto assignmentDto)
        {
            try
            {
                // Verify the company exists
                var companyExists = await _context.Companies.AnyAsync(c => c.Id == assignmentDto.CompanyID);
                if (!companyExists)
                {
                    throw new KeyNotFoundException($"Company with ID {assignmentDto.CompanyID} not found");
                }

                // Handle create or update based on Id value
                if (assignmentDto.Id == 0)
                {
                    // Create new assignment with ID from IdPool
                    int newId = await _idPoolService.GetNextIdAsync(IdPoolType.Assignment);

                    var assignment = new Assignment
                    {
                        Id = newId,
                        CompanyID = assignmentDto.CompanyID,
                        AssignmentTypeCode = assignmentDto.AssignmentTypeCode,
                        EmployeeID = assignmentDto.EmployeeID,
                        OpeningEffecDate = assignmentDto.OpeningEffecDate,
                        ClosingEffecDate = assignmentDto.ClosingEffecDate,
                        OpeningRegDate = assignmentDto.OpeningRegDate,
                        ClosingRegDate = assignmentDto.ClosingRegDate,
                        OpeningRef = assignmentDto.OpeningRef,
                        ClosingRef = assignmentDto.ClosingRef,
                        ActiveFlag = assignmentDto.ActiveFlag
                    };

                    _context.Assignments.Add(assignment);
                    await _context.SaveChangesAsync();

                    // Get the company name for the response
                    var company = await _context.Companies.FindAsync(assignmentDto.CompanyID);

                    assignmentDto.Id = newId;
                    assignmentDto.CompanyName = company.RegistrationName;
                }
                else
                {
                    // Update existing assignment
                    var assignment = await _context.Assignments.FindAsync(assignmentDto.Id);
                    if (assignment == null)
                    {
                        throw new KeyNotFoundException($"Assignment with ID {assignmentDto.Id} not found");
                    }

                    // Update assignment properties
                    assignment.CompanyID = assignmentDto.CompanyID;
                    assignment.AssignmentTypeCode = assignmentDto.AssignmentTypeCode;
                    assignment.EmployeeID = assignmentDto.EmployeeID;
                    assignment.OpeningEffecDate = assignmentDto.OpeningEffecDate;
                    assignment.ClosingEffecDate = assignmentDto.ClosingEffecDate;
                    assignment.OpeningRegDate = assignmentDto.OpeningRegDate;
                    assignment.ClosingRegDate = assignmentDto.ClosingRegDate;
                    assignment.OpeningRef = assignmentDto.OpeningRef;
                    assignment.ClosingRef = assignmentDto.ClosingRef;
                    assignment.ActiveFlag = assignmentDto.ActiveFlag;

                    _context.Entry(assignment).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    // Get the company name for the response
                    var company = await _context.Companies.FindAsync(assignmentDto.CompanyID);
                    assignmentDto.CompanyName = company.RegistrationName;
                }

                return assignmentDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving assignment");
                throw;
            }
        }

        public async Task DeleteAssignmentAsync(int id)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(id);
                if (assignment == null)
                {
                    throw new KeyNotFoundException($"Assignment with ID {id} not found");
                }

                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting assignment with id {Id}", id);
                throw;
            }
        }

        public async Task<bool> AssignmentExistsAsync(int id)
        {
            return await _context.Assignments.AnyAsync(e => e.Id == id);
        }
    }
}
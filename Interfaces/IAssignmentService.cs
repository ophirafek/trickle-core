// Interfaces/IAssignmentService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ACIA.DTOs;

namespace ACIA.Services
{
    public interface IAssignmentService
    {
        Task<IEnumerable<AssignmentDto>> GetAssignmentsAsync();
        Task<IEnumerable<AssignmentDto>> GetAssignmentsByCompanyAsync(int companyId);
        Task<IEnumerable<AssignmentDto>> GetAssignmentsByEmployeeAsync(int employeeId);
        Task<AssignmentDto> GetAssignmentByIdAsync(int id);
        Task<AssignmentDto> SaveAssignmentAsync(AssignmentDto assignmentDto);
        Task DeleteAssignmentAsync(int id);
        Task<bool> AssignmentExistsAsync(int id);
    }
}
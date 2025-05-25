// Interfaces/ILeadService.cs - Updated for new lead structure
using System.Collections.Generic;
using System.Threading.Tasks;
using ACIA.DTOs;

namespace ACIA.Services
{
    public interface ILeadService
    {
        /// <summary>
        /// Gets all active leads
        /// </summary>
        /// <returns>Collection of lead DTOs</returns>
        Task<IEnumerable<LeadDto>> GetLeadsAsync();

        /// <summary>
        /// Gets all active leads for a specific company
        /// </summary>
        /// <param name="companyId">The company ID</param>
        /// <returns>Collection of lead DTOs for the company</returns>
        Task<IEnumerable<LeadDto>> GetLeadsByCompanyAsync(int companyId);

        /// <summary>
        /// Gets a specific lead by its ID
        /// </summary>
        /// <param name="id">The lead ID</param>
        /// <returns>Lead DTO or null if not found</returns>
        Task<LeadDto> GetLeadByIdAsync(int id);

        /// <summary>
        /// Creates a new lead
        /// </summary>
        /// <param name="leadDto">Lead creation data</param>
        /// <returns>Created lead DTO</returns>
        Task<LeadDto> CreateLeadAsync(LeadCreateDto leadDto);

        /// <summary>
        /// Updates an existing lead (creates new version and deactivates old)
        /// </summary>
        /// <param name="id">The lead ID to update</param>
        /// <param name="leadDto">Updated lead data</param>
        Task UpdateLeadAsync(int id, LeadUpdateDto leadDto);

        /// <summary>
        /// Soft deletes a lead (marks as inactive)
        /// </summary>
        /// <param name="id">The lead ID to delete</param>
        Task DeleteLeadAsync(int id);

        /// <summary>
        /// Checks if a lead exists and is active
        /// </summary>
        /// <param name="id">The lead ID</param>
        /// <returns>True if lead exists and is active</returns>
        Task<bool> LeadExistsAsync(int id);

        /// <summary>
        /// Gets leads by owner employee ID
        /// </summary>
        /// <param name="ownerEmployeeId">The owner employee ID</param>
        /// <returns>Collection of lead DTOs owned by the employee</returns>
        Task<IEnumerable<LeadDto>> GetLeadsByOwnerAsync(int ownerEmployeeId);

        /// <summary>
        /// Gets leads by status code
        /// </summary>
        /// <param name="statusCode">The status code</param>
        /// <returns>Collection of lead DTOs with the specified status</returns>
        Task<IEnumerable<LeadDto>> GetLeadsByStatusAsync(string statusCode);

        /// <summary>
        /// Gets leads by market code
        /// </summary>
        /// <param name="marketCode">The market code</param>
        /// <returns>Collection of lead DTOs for the specified market</returns>
        Task<IEnumerable<LeadDto>> GetLeadsByMarketAsync(int marketCode);

        /// <summary>
        /// Gets leads by contact ID
        /// </summary>
        /// <param name="contactId">The contact ID</param>
        /// <returns>Collection of lead DTOs associated with the contact</returns>
        Task<IEnumerable<LeadDto>> GetLeadsByContactAsync(int contactId);
    }
}
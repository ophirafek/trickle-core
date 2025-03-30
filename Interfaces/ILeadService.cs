using System.Collections.Generic;
using System.Threading.Tasks;
using LeadManagerPro.DTOs;

namespace LeadManagerPro.Services
{
    public interface ILeadService
    {
        Task<IEnumerable<LeadDto>> GetLeadsAsync();
        Task<IEnumerable<LeadDto>> GetLeadsByCompanyAsync(int companyId);
        Task<LeadDto> GetLeadByIdAsync(int id);
        Task<LeadDto> CreateLeadAsync(LeadCreateDto leadDto);
        Task UpdateLeadAsync(int id, LeadUpdateDto leadDto);
        Task DeleteLeadAsync(int id);
        Task<bool> LeadExistsAsync(int id);
    }
}
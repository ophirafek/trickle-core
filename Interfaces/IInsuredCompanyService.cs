// Interfaces/IInsuredCompanyService.cs
using System.Threading.Tasks;
using ACIA.DTOs;

namespace ACIA.Services
{
    public interface IInsuredCompanyService
    {
        Task<InsuredCompanyDto> SaveInsuredCompanyAsync(InsuredCompanyDto insuredCompanyDto);
        Task DeleteInsuredCompanyAsync(int id);
        Task<bool> InsuredCompanyExistsAsync(int id);
    }
}
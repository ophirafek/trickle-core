using System.Collections.Generic;
using System.Threading.Tasks;
using LeadManagerPro.DTOs;
using LeadManagerPro.Models;

namespace LeadManagerPro.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetCompaniesAsync();
        Task<CompanyDto> GetCompanyByIdAsync(int id);
        Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto);
        Task UpdateCompanyAsync(int id, CompanyDto companyDto);
        Task DeleteCompanyAsync(int id);
        Task<bool> CompanyExistsAsync(int id);
        Task<ContactDto> AddContactAsync(int companyId, ContactCreateDto contactDto);
        Task<NoteDto> AddNoteAsync(int companyId, NoteCreateDto noteDto);
    }
}
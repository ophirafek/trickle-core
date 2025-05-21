using System.Collections.Generic;
using System.Threading.Tasks;
using ACIA.DTOs;

namespace ACIA.Services
{
    public interface IContactService
    {
        Task<ContactDto> GetContactByIdAsync(int id);
        Task<IEnumerable<ContactDto>> GetContactsByCompanyAsync(int companyId);
        Task<ContactDto> SaveContactAsync(ContactDto contactDto);
        Task DeleteContactAsync(int id);
        Task<bool> ContactExistsAsync(int id);
        Task<bool> ActiveContactExistsAsync(int id);
    }
}
using System.Threading.Tasks;
using LeadManagerPro.DTOs;

namespace LeadManagerPro.Services
{
    public interface IContactService
    {
        Task<ContactDto> GetContactByIdAsync(int id);
        Task UpdateContactAsync(int id, ContactUpdateDto contactDto);
        Task DeleteContactAsync(int id);
        Task<bool> ContactExistsAsync(int id);
    }
}
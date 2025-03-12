using System.Threading.Tasks;
using LeadManagerPro.DTOs;

namespace LeadManagerPro.Services
{
    public interface INoteService
    {
        Task<NoteDto> GetNoteByIdAsync(int id);
        Task UpdateNoteAsync(int id, NoteUpdateDto noteDto);
        Task DeleteNoteAsync(int id);
        Task<bool> NoteExistsAsync(int id);
    }
}
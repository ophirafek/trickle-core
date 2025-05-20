using System.Threading.Tasks;
using ACIA.DTOs;

namespace ACIA.Services
{
    public interface INoteService
    {
        Task<NoteDto> GetNoteByIdAsync(int id);
        Task UpdateNoteAsync(int id, NoteUpdateDto noteDto);
        Task DeleteNoteAsync(int id);
        Task<bool> NoteExistsAsync(int id);
    }
}
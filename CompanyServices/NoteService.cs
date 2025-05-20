using System;
using System.Threading.Tasks;
using ACIA.Data;
using ACIA.DTOs;
using ACIA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ACIA.Services
{
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NoteService> _logger;

        public NoteService(ApplicationDbContext context, ILogger<NoteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<NoteDto> GetNoteByIdAsync(int id)
        {
            try
            {
                var note = await _context.Notes.FindAsync(id);

                if (note == null)
                {
                    return null;
                }

                var noteDto = new NoteDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    Content = note.Content,
                    CreatedAt = note.CreatedAt,
                    CompanyId = note.CompanyId
                };

                return noteDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting note with id {Id}", id);
                throw;
            }
        }

        public async Task UpdateNoteAsync(int id, NoteUpdateDto noteDto)
        {
            try
            {
                var note = await _context.Notes.FindAsync(id);
                if (note == null)
                {
                    throw new KeyNotFoundException($"Note with ID {id} not found");
                }

                // Update note properties
                note.Title = noteDto.Title;
                note.Content = noteDto.Content;
                // Note: We don't update CompanyId here to prevent moving notes between companies

                _context.Entry(note).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating note with id {Id}", id);
                throw;
            }
        }

        public async Task DeleteNoteAsync(int id)
        {
            try
            {
                var note = await _context.Notes.FindAsync(id);
                if (note == null)
                {
                    throw new KeyNotFoundException($"Note with ID {id} not found");
                }

                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting note with id {Id}", id);
                throw;
            }
        }

        public async Task<bool> NoteExistsAsync(int id)
        {
            return await _context.Notes.AnyAsync(e => e.Id == id);
        }
    }
}
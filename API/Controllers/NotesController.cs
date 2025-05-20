using System;
using System.Threading.Tasks;
using ACIA.DTOs;
using ACIA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACIA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly ILogger<NotesController> _logger;

        public NotesController(INoteService noteService, ILogger<NotesController> logger)
        {
            _noteService = noteService;
            _logger = logger;
        }

        // GET: api/Notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NoteDto>> GetNote(int id)
        {
            try
            {
                var noteDto = await _noteService.GetNoteByIdAsync(id);

                if (noteDto == null)
                {
                    return NotFound();
                }

                return noteDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting note with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/Notes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, NoteUpdateDto noteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!await _noteService.NoteExistsAsync(id))
                {
                    return NotFound();
                }

                await _noteService.UpdateNoteAsync(id, noteDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating note with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/Notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            try
            {
                if (!await _noteService.NoteExistsAsync(id))
                {
                    return NotFound();
                }

                await _noteService.DeleteNoteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting note with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
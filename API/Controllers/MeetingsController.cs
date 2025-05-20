using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACIA.DTOs;
using ACIA.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACIA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly IMeetingService _meetingService;
        private readonly ILogger<MeetingsController> _logger;

        public MeetingsController(IMeetingService meetingService, ILogger<MeetingsController> logger)
        {
            _meetingService = meetingService;
            _logger = logger;
        }

        // GET: api/Meetings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeetingDto>>> GetMeetings()
        {
            try
            {
                var meetings = await _meetingService.GetMeetingsAsync();
                return Ok(meetings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting meetings");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Meetings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MeetingDto>> GetMeeting(int id)
        {
            try
            {
                var meeting = await _meetingService.GetMeetingByIdAsync(id);
                if (meeting == null)
                {
                    return NotFound();
                }

                return Ok(meeting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting meeting with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/Meetings
        [HttpPost]
        public async Task<ActionResult<MeetingDto>> CreateMeeting(MeetingCreateDto meetingDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newMeeting = await _meetingService.CreateMeetingAsync(meetingDto);
                return CreatedAtAction(nameof(GetMeeting), new { id = newMeeting.Id }, newMeeting);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating meeting");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/Meetings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeeting(int id, MeetingUpdateDto meetingDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!await _meetingService.MeetingExistsAsync(id))
                {
                    return NotFound();
                }

                await _meetingService.UpdateMeetingAsync(id, meetingDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating meeting with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/Meetings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeeting(int id)
        {
            try
            {
                if (!await _meetingService.MeetingExistsAsync(id))
                {
                    return NotFound();
                }

                await _meetingService.DeleteMeetingAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting meeting with id {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/Meetings/{meetingId}/Attendees
        [HttpPost("{meetingId}/attendees")]
        public async Task<ActionResult<AttendeeDto>> AddAttendee(int meetingId, AttendeeCreateDto attendeeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verify the meeting exists
                if (!await _meetingService.MeetingExistsAsync(meetingId))
                {
                    return NotFound($"Meeting with ID {meetingId} not found");
                }

                var newAttendee = await _meetingService.AddAttendeeAsync(meetingId, attendeeDto);
                return CreatedAtAction(nameof(GetMeeting), new { id = meetingId }, newAttendee);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding attendee to meeting with id {MeetingId}", meetingId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/Meetings/Attendees/5
        [HttpPut("attendees/{attendeeId}")]
        public async Task<IActionResult> UpdateAttendee(int attendeeId, AttendeeUpdateDto attendeeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _meetingService.UpdateAttendeeAsync(attendeeId, attendeeDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating attendee with id {AttendeeId}", attendeeId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/Meetings/Attendees/5
        [HttpDelete("attendees/{attendeeId}")]
        public async Task<IActionResult> DeleteAttendee(int attendeeId)
        {
            try
            {
                await _meetingService.DeleteAttendeeAsync(attendeeId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting attendee with id {AttendeeId}", attendeeId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
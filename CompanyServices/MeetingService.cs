using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACIA.Data;
using ACIA.DTOs;
using ACIA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ACIA.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MeetingService> _logger;

        public MeetingService(ApplicationDbContext context, ILogger<MeetingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<MeetingDto>> GetMeetingsAsync()
        {
            try
            {
                var meetings = await _context.Meetings
                    .Include(m => m.Company)
                    .Include(m => m.Attendees)
                    .Select(m => new MeetingDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Type = m.Type,
                        CompanyId = m.CompanyId,
                        CompanyName = m.Company.RegistrationName,
                        Date = m.Date,
                        Time = m.Time,
                        Duration = m.Duration,
                        Status = m.Status,
                        Location = m.Location,
                        Description = m.Description,
                        CreatedBy = m.CreatedBy,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt,
                        Attendees = m.Attendees.Select(a => new AttendeeDto
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Role = a.Role,
                            Company = a.Company,
                            MeetingId = a.MeetingId
                        }).ToList()
                    })
                    .ToListAsync();

                return meetings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting meetings");
                throw;
            }
        }

        public async Task<MeetingDto> GetMeetingByIdAsync(int id)
        {
            try
            {
                var meeting = await _context.Meetings
                    .Include(m => m.Company)
                    .Include(m => m.Attendees)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (meeting == null)
                {
                    return null;
                }

                var meetingDto = new MeetingDto
                {
                    Id = meeting.Id,
                    Title = meeting.Title,
                    Type = meeting.Type,
                    CompanyId = meeting.CompanyId,
                    CompanyName = meeting.Company.RegistrationName,
                    Date = meeting.Date,
                    Time = meeting.Time,
                    Duration = meeting.Duration,
                    Status = meeting.Status,
                    Location = meeting.Location,
                    Description = meeting.Description,
                    CreatedBy = meeting.CreatedBy,
                    CreatedAt = meeting.CreatedAt,
                    UpdatedAt = meeting.UpdatedAt,
                    Attendees = meeting.Attendees.Select(a => new AttendeeDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Role = a.Role,
                        Company = a.Company,
                        MeetingId = a.MeetingId
                    }).ToList()
                };

                return meetingDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting meeting with id {Id}", id);
                throw;
            }
        }

        public async Task<MeetingDto> CreateMeetingAsync(MeetingCreateDto meetingDto)
        {
            try
            {
                // Verify the company exists
                var companyExists = await _context.Companies.AnyAsync(c => c.Id == meetingDto.CompanyId);
                if (!companyExists)
                {
                    throw new KeyNotFoundException($"Company with ID {meetingDto.CompanyId} not found");
                }

                var meeting = new Meeting
                {
                    Title = meetingDto.Title,
                    Type = meetingDto.Type,
                    CompanyId = meetingDto.CompanyId,
                    Date = meetingDto.Date,
                    Time = meetingDto.Time,
                    Duration = meetingDto.Duration,
                    Status = meetingDto.Status,
                    Location = meetingDto.Location,
                    Description = meetingDto.Description,
                    CreatedBy = meetingDto.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    Attendees = meetingDto.Attendees.Select(a => new MeetingAttendee
                    {
                        Name = a.Name,
                        Role = a.Role,
                        Company = a.Company
                    }).ToList()
                };

                _context.Meetings.Add(meeting);
                await _context.SaveChangesAsync();

                // Get the company name for the response
                var company = await _context.Companies.FindAsync(meetingDto.CompanyId);

                var newMeetingDto = new MeetingDto
                {
                    Id = meeting.Id,
                    Title = meeting.Title,
                    Type = meeting.Type,
                    CompanyId = meeting.CompanyId,
                    CompanyName = company. RegistrationName,
                    Date = meeting.Date,
                    Time = meeting.Time,
                    Duration = meeting.Duration,
                    Status = meeting.Status,
                    Location = meeting.Location,
                    Description = meeting.Description,
                    CreatedBy = meeting.CreatedBy,
                    CreatedAt = meeting.CreatedAt,
                    UpdatedAt = meeting.UpdatedAt,
                    Attendees = meeting.Attendees.Select(a => new AttendeeDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Role = a.Role,
                        Company = a.Company,
                        MeetingId = a.MeetingId
                    }).ToList()
                };

                return newMeetingDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating meeting");
                throw;
            }
        }

        public async Task UpdateMeetingAsync(int id, MeetingUpdateDto meetingDto)
        {
            try
            {
                var meeting = await _context.Meetings
                    .Include(m => m.Attendees)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (meeting == null)
                {
                    throw new KeyNotFoundException($"Meeting with ID {id} not found");
                }

                // Verify the company exists if it's being changed
                if (meeting.CompanyId != meetingDto.CompanyId)
                {
                    var companyExists = await _context.Companies.AnyAsync(c => c.Id == meetingDto.CompanyId);
                    if (!companyExists)
                    {
                        throw new KeyNotFoundException($"Company with ID {meetingDto.CompanyId} not found");
                    }
                }

                // Update meeting properties
                meeting.Title = meetingDto.Title;
                meeting.Type = meetingDto.Type;
                meeting.CompanyId = meetingDto.CompanyId;
                meeting.Date = meetingDto.Date;
                meeting.Time = meetingDto.Time;
                meeting.Duration = meetingDto.Duration;
                meeting.Status = meetingDto.Status;
                meeting.Location = meetingDto.Location;
                meeting.Description = meetingDto.Description;
                meeting.UpdatedAt = DateTime.UtcNow;

                // Handle attendees update - remove existing ones and add new ones
                _context.MeetingAttendees.RemoveRange(meeting.Attendees);

                meeting.Attendees = meetingDto.Attendees.Select(a => new MeetingAttendee
                {
                    Name = a.Name,
                    Role = a.Role,
                    Company = a.Company,
                    MeetingId = meeting.Id
                }).ToList();

                _context.Entry(meeting).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating meeting with id {Id}", id);
                throw;
            }
        }

        public async Task DeleteMeetingAsync(int id)
        {
            try
            {
                var meeting = await _context.Meetings
                    .Include(m => m.Attendees)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (meeting == null)
                {
                    throw new KeyNotFoundException($"Meeting with ID {id} not found");
                }

                _context.MeetingAttendees.RemoveRange(meeting.Attendees);
                _context.Meetings.Remove(meeting);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting meeting with id {Id}", id);
                throw;
            }
        }

        public async Task<bool> MeetingExistsAsync(int id)
        {
            return await _context.Meetings.AnyAsync(e => e.Id == id);
        }

        public async Task<AttendeeDto> AddAttendeeAsync(int meetingId, AttendeeCreateDto attendeeDto)
        {
            try
            {
                // Verify the meeting exists
                var meetingExists = await _context.Meetings.AnyAsync(m => m.Id == meetingId);
                if (!meetingExists)
                {
                    throw new KeyNotFoundException($"Meeting with ID {meetingId} not found");
                }

                var attendee = new MeetingAttendee
                {
                    Name = attendeeDto.Name,
                    Role = attendeeDto.Role,
                    Company = attendeeDto.Company,
                    MeetingId = meetingId
                };

                _context.MeetingAttendees.Add(attendee);
                await _context.SaveChangesAsync();

                var newAttendeeDto = new AttendeeDto
                {
                    Id = attendee.Id,
                    Name = attendee.Name,
                    Role = attendee.Role,
                    Company = attendee.Company,
                    MeetingId = attendee.MeetingId
                };

                return newAttendeeDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding attendee to meeting with id {MeetingId}", meetingId);
                throw;
            }
        }

        public async Task UpdateAttendeeAsync(int attendeeId, AttendeeUpdateDto attendeeDto)
        {
            try
            {
                var attendee = await _context.MeetingAttendees.FindAsync(attendeeId);
                if (attendee == null)
                {
                    throw new KeyNotFoundException($"Attendee with ID {attendeeId} not found");
                }

                // Update attendee properties
                attendee.Name = attendeeDto.Name;
                attendee.Role = attendeeDto.Role;
                attendee.Company = attendeeDto.Company;

                _context.Entry(attendee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating attendee with id {AttendeeId}", attendeeId);
                throw;
            }
        }

        public async Task DeleteAttendeeAsync(int attendeeId)
        {
            try
            {
                var attendee = await _context.MeetingAttendees.FindAsync(attendeeId);
                if (attendee == null)
                {
                    throw new KeyNotFoundException($"Attendee with ID {attendeeId} not found");
                }

                _context.MeetingAttendees.Remove(attendee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting attendee with id {AttendeeId}", attendeeId);
                throw;
            }
        }
    }
}
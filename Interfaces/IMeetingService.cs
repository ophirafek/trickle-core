using System.Collections.Generic;
using System.Threading.Tasks;
using LeadManagerPro.DTOs;

namespace LeadManagerPro.Services
{
    public interface IMeetingService
    {
        Task<IEnumerable<MeetingDto>> GetMeetingsAsync();
        Task<MeetingDto> GetMeetingByIdAsync(int id);
        Task<MeetingDto> CreateMeetingAsync(MeetingCreateDto meetingDto);
        Task UpdateMeetingAsync(int id, MeetingUpdateDto meetingDto);
        Task DeleteMeetingAsync(int id);
        Task<bool> MeetingExistsAsync(int id);
        Task<AttendeeDto> AddAttendeeAsync(int meetingId, AttendeeCreateDto attendeeDto);
        Task UpdateAttendeeAsync(int attendeeId, AttendeeUpdateDto attendeeDto);
        Task DeleteAttendeeAsync(int attendeeId);
    }
}
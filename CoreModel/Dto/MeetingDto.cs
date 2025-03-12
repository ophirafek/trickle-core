using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeadManagerPro.DTOs
{
    public class MeetingDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Duration { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<AttendeeDto> Attendees { get; set; }
    }

    public class MeetingCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(20)]
        public string Time { get; set; }

        [StringLength(50)]
        public string Duration { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Upcoming";

        [StringLength(200)]
        public string Location { get; set; }

        public string Description { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public ICollection<AttendeeCreateDto> Attendees { get; set; } = new List<AttendeeCreateDto>();
    }

    public class MeetingUpdateDto : MeetingCreateDto
    {
        // Uses all properties from MeetingCreateDto
    }

    public class AttendeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Company { get; set; }
        public int MeetingId { get; set; }
    }

    public class AttendeeCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Role { get; set; }

        [StringLength(100)]
        public string Company { get; set; }
    }

    public class AttendeeUpdateDto : AttendeeCreateDto
    {
        // Uses all properties from AttendeeCreateDto
    }
}
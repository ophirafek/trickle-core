using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeadManagerPro.Models
{
    public class Meeting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<MeetingAttendee> Attendees { get; set; }
    }

    public class MeetingAttendee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Role { get; set; }

        [StringLength(100)]
        public string Company { get; set; }

        public int MeetingId { get; set; }

        [ForeignKey("MeetingId")]
        public Meeting Meeting { get; set; }
    }
}
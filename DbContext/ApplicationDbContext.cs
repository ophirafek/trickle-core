using ACIA.Models;
using Microsoft.EntityFrameworkCore;

namespace ACIA.Data
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingAttendee> MeetingAttendees { get; set; }
        // Addition to DbContext/ApplicationDbContext.cs
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<InsuredCompany> InsuredCompanies { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Contacts)
                .WithOne(c => c.Company)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Company>()
                .HasMany(c => c.Notes)
                .WithOne(c => c.Company)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Lead relationship with Company
            modelBuilder.Entity<Lead>()
                .HasOne(l => l.Company)
                .WithMany()
                .HasForeignKey(l => l.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Meeting relationship with Company
            modelBuilder.Entity<Meeting>()
                .HasOne(m => m.Company)
                .WithMany()
                .HasForeignKey(m => m.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Meeting relationship with MeetingAttendees
            modelBuilder.Entity<Meeting>()
                .HasMany(m => m.Attendees)
                .WithOne(a => a.Meeting)
                .HasForeignKey(a => a.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);
            // Configure Assignment relationship with Company
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Company)
                .WithMany()
                .HasForeignKey(a => a.CompanyID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure InsuredCompany relationship with Company
            modelBuilder.Entity<InsuredCompany>()
                .HasOne(i => i.Company)
                .WithMany()
                .HasForeignKey(i => i.CompanyID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using api.Entities;

namespace api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<PaymentLog> PaymentLogs { get; set; }
    public DbSet<AttendanceLog> AttendanceLogs { get; set; }
    public DbSet<StudentMeasurement> StudentMeasurements { get; set; }
    public DbSet<ScheduleSlot> ScheduleSlots { get; set; }
    public DbSet<ScheduleEnrollment> ScheduleEnrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => u.IsActive);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique()
            .HasFilter("\"IsActive\" = true");

        modelBuilder.Entity<PaymentLog>()
            .HasOne(p => p.Student)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AttendanceLog>()
            .HasOne(a => a.Student)
            .WithMany(u => u.Attendances)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentMeasurement>()
            .HasOne(m => m.Student)
            .WithMany()
            .HasForeignKey(m => m.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentMeasurement>()
            .HasIndex(m => m.StudentId)
            .IsUnique();

        modelBuilder.Entity<ScheduleSlot>()
            .HasIndex(s => new { s.DayOfWeek, s.StartTime })
            .IsUnique();

        modelBuilder.Entity<ScheduleEnrollment>()
            .HasQueryFilter(e => e.Student.IsActive);

        modelBuilder.Entity<ScheduleEnrollment>()
            .HasIndex(e => new { e.ScheduleSlotId, e.StudentId })
            .IsUnique();

        modelBuilder.Entity<ScheduleEnrollment>()
            .HasOne(e => e.ScheduleSlot)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.ScheduleSlotId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ScheduleEnrollment>()
            .HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
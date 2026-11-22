namespace api.Entities;

public class ScheduleEnrollment
{
    public int Id { get; set; }

    public int ScheduleSlotId { get; set; }
    public ScheduleSlot ScheduleSlot { get; set; } = null!;

    public int StudentId { get; set; }
    public User Student { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
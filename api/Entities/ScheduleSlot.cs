namespace api.Entities;

public class ScheduleSlot
{
    public int Id { get; set; }
    public int DayOfWeek { get; set; } 
    public string StartTime { get; set; } = string.Empty; 
    public int Capacity { get; set; } = 4;

    public List<ScheduleEnrollment> Enrollments { get; set; } = [];
}
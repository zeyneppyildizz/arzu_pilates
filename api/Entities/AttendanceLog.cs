namespace api.Entities;
public class AttendanceLog
{
    public int Id { get; set; }   
    public int StudentId { get; set; }
    public User Student { get; set; } = null!;
    public DateTime AttendedAt { get; set; } = DateTime.UtcNow;
    public int BalanceAfterAttendance { get; set; } 
}
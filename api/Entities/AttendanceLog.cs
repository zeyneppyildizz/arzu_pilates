namespace api.Entities;
public enum AttendanceType
{
    QrScan = 0,
    ManualAttendance = 1,
    LateCancellation = 2
}
public class AttendanceLog
{
    public int Id { get; set; }   
    public int StudentId { get; set; }
    public User Student { get; set; } = null!;
    public DateTime AttendedAt { get; set; } = DateTime.UtcNow;
    public int BalanceAfterAttendance { get; set; }
    public AttendanceType Type { get; set; } = AttendanceType.QrScan;
    public string? Note { get; set; }
}
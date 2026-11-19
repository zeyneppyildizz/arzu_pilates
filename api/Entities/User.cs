namespace api.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;  
    public UserRole Role { get; set; }
    public int Balance { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public ICollection<PaymentLog> Payments { get; set; } = [];
    public ICollection<AttendanceLog> Attendances { get; set; } = [];
}

public enum UserRole
{
    Student = 0,
    Instructor = 1
}
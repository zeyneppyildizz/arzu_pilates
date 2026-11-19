namespace api.Entities;
public class PaymentLog
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public User Student { get; set; } = null!;
    public int InstructorId { get; set; } 
    public int AddedSessions { get; set; }  
    public decimal Amount { get; set; } 
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
}
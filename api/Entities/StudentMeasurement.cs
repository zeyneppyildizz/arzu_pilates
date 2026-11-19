namespace api.Entities;

public class StudentMeasurement
{
    public int Id { get; set; }
    public int StudentId { get; set; }

    public double? Height { get; set; }
    public double? Weight { get; set; }
    public double? Waist { get; set; }
    public double? Abdomen { get; set; }
    public double? Hips { get; set; }
    public double? RightLeg { get; set; }
    public double? LeftLeg { get; set; }
    public double? RightArm { get; set; }
    public double? LeftArm { get; set; }

    public User Student { get; set; } = null!;
}
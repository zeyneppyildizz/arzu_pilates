using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public record CreateScheduleSlotRequest(
    [Range(1, 7, ErrorMessage = "Geçerli bir gün seçiniz.")]
    int DayOfWeek,

    [Required(ErrorMessage = "Saat zorunludur.")]
    [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "Saat formatı SS:DD olmalıdır (örn. 08:00).")]
    string StartTime
);

public record AssignStudentRequest(int StudentId);

public record ScheduleStudentDto(
    int Id,
    string FirstName,
    string LastName,
    string Username,
    int Balance
);

public record ScheduleSlotDto(
    int Id,
    int DayOfWeek,
    string StartTime,
    int Capacity,
    List<ScheduleStudentDto> Students
);

public record MyScheduleSlotDto(
    int Id,
    int DayOfWeek,
    string StartTime
);
using api.Entities;

namespace api.DTOs;

public record AttendanceDto(int Id, DateTime Date, int BalanceAfter, AttendanceType Type, string? Note);

public record PaymentDto(int Id, DateTime Date, int AddedSessions, decimal Amount);

public record StudentHistoryResponse(List<AttendanceDto> Attendances, List<PaymentDto> Payments);
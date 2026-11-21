using System.ComponentModel.DataAnnotations;
using api.Entities;

namespace api.DTOs;

public record ScanQrRequest(string Token);

public record QrPayload
{
    public int Id { get; init; }
    public long Timestamp { get; init; }
}

public record ManualDeductRequest(
    int StudentId,
    AttendanceType Type,
    [StringLength(200, ErrorMessage = "Not en fazla 200 karakter olabilir.")]
    string? Note
);
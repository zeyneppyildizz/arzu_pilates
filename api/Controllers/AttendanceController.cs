using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using api.Data;
using api.Entities;
using api.DTOs;
using api.Hubs;

namespace api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttendanceController(AppDbContext _context, IHubContext<AttendanceHub> _hubContext) : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Authorize(Roles = "1")]
    [HttpGet("today-summary")]
    public async Task<IActionResult> GetTodaySummary()
    {
        var timeZoneId = OperatingSystem.IsWindows() ? "Turkey Standard Time" : "Europe/Istanbul";
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var localDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).Date;
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(localDate, timeZone);
        var endUtc = TimeZoneInfo.ConvertTimeToUtc(localDate.AddDays(1), timeZone);

        var attendanceCount = await _context.AttendanceLogs
            .CountAsync(log => log.AttendedAt >= startUtc && log.AttendedAt < endUtc);

        return Ok(new { count = attendanceCount });
    }

    [Authorize(Roles = "1")]
    [HttpPost("scan")]
    public async Task<IActionResult> ScanQr([FromBody] ScanQrRequest request)
    {
        QrPayload? payload;
        try
        {
            var base64Bytes = Convert.FromBase64String(request.Token);
            var jsonString = Encoding.UTF8.GetString(base64Bytes);
            payload = JsonSerializer.Deserialize<QrPayload>(jsonString, JsonOptions);
        }
        catch (Exception ex) when (ex is FormatException or JsonException or ArgumentNullException)
        {
            return BadRequest(new { Message = "Geçersiz karekod formatı!" });
        }

        if (payload is null)
            return BadRequest(new { Message = "Geçersiz karekod formatı!" });

        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var timeDifference = currentTime - payload.Timestamp;
        if (timeDifference > 605000 || timeDifference < -5000)
        {
            return BadRequest(new { Message = "Karekodun süresi dolmuş. Lütfen öğrencinin ekranı yenilemesini bekleyin." });
        }

        var hasRecentAttendance = await _context.AttendanceLogs
            .AnyAsync(a => a.StudentId == payload.Id && a.AttendedAt >= DateTime.UtcNow.AddMinutes(-1));

        if (hasRecentAttendance)
        {
            return BadRequest(new { Message = "Bu karekod zaten okutuldu!" });
        }

        var student = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == payload.Id && u.Role == UserRole.Student);

        if (student == null)
            return NotFound(new { Message = "Öğrenci bulunamadı." });

        student.Balance -= 1;

        var log = new AttendanceLog
        {
            StudentId = student.Id,
            AttendedAt = DateTime.UtcNow,
            BalanceAfterAttendance = student.Balance
        };

        _context.AttendanceLogs.Add(log);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveAttendanceUpdate", student.Id, student.Balance);

        return Ok(new
        {
            Message = "Yoklama başarıyla alındı.",
            NewBalance = student.Balance
        });
    }
}
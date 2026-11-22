using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.DTOs;
using api.Entities;

namespace api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScheduleController(AppDbContext _context) : ControllerBase
{
    [Authorize(Roles = "1")]
    [HttpGet]
    public async Task<IActionResult> GetSchedule()
    {
        var slots = await _context.ScheduleSlots
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Student)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToListAsync();

        var result = slots.Select(s => new ScheduleSlotDto(
            s.Id,
            s.DayOfWeek,
            s.StartTime,
            s.Capacity,
            s.Enrollments
                .OrderBy(e => e.CreatedAt)
                .Select(e => new ScheduleStudentDto(
                    e.Student.Id,
                    e.Student.FirstName,
                    e.Student.LastName,
                    e.Student.Username,
                    e.Student.Balance
                ))
                .ToList()
        )).ToList();

        return Ok(result);
    }

    [Authorize(Roles = "0")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMySchedule()
    {
        var currentUserId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"
        );

        var mySlots = await _context.ScheduleEnrollments
            .Where(e => e.StudentId == currentUserId)
            .OrderBy(e => e.ScheduleSlot.DayOfWeek)
            .ThenBy(e => e.ScheduleSlot.StartTime)
            .Select(e => new MyScheduleSlotDto(
                e.ScheduleSlot.Id,
                e.ScheduleSlot.DayOfWeek,
                e.ScheduleSlot.StartTime
            ))
            .ToListAsync();

        return Ok(mySlots);
    }

    [Authorize(Roles = "1")]
    [HttpPost("slots")]
    public async Task<IActionResult> CreateSlot([FromBody] CreateScheduleSlotRequest request)
    {
        var startTime = request.StartTime.Trim();

        var exists = await _context.ScheduleSlots
            .AnyAsync(s => s.DayOfWeek == request.DayOfWeek && s.StartTime == startTime);

        if (exists)
            return BadRequest(new { message = "Bu gün ve saat için zaten bir ders kartı var." });

        var slot = new ScheduleSlot
        {
            DayOfWeek = request.DayOfWeek,
            StartTime = startTime,
            Capacity = 4
        };

        _context.ScheduleSlots.Add(slot);
        await _context.SaveChangesAsync();

        return Ok(new ScheduleSlotDto(slot.Id, slot.DayOfWeek, slot.StartTime, slot.Capacity, []));
    }

    [Authorize(Roles = "1")]
    [HttpDelete("slots/{slotId}")]
    public async Task<IActionResult> DeleteSlot(int slotId)
    {
        var slot = await _context.ScheduleSlots.FirstOrDefaultAsync(s => s.Id == slotId);
        if (slot == null)
            return NotFound(new { message = "Ders saati bulunamadı." });

        _context.ScheduleSlots.Remove(slot);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Ders saati silindi." });
    }

    [Authorize(Roles = "1")]
    [HttpPost("slots/{slotId}/students")]
    public async Task<IActionResult> AddStudentToSlot(int slotId, [FromBody] AssignStudentRequest request)
    {
        var slot = await _context.ScheduleSlots
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Id == slotId);

        if (slot == null)
            return NotFound(new { message = "Ders saati bulunamadı." });

        var student = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.StudentId && u.Role == UserRole.Student);

        if (student == null)
            return NotFound(new { message = "Öğrenci bulunamadı." });

        if (slot.Enrollments.Any(e => e.StudentId == request.StudentId))
            return BadRequest(new { message = "Bu öğrenci bu derse zaten ekli." });

        if (slot.Enrollments.Count >= slot.Capacity)
            return BadRequest(new { message = $"Bu ders saati dolu ({slot.Capacity}/{slot.Capacity})." });

        var enrollment = new ScheduleEnrollment
        {
            ScheduleSlotId = slotId,
            StudentId = student.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.ScheduleEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        return Ok(new ScheduleStudentDto(
            student.Id,
            student.FirstName,
            student.LastName,
            student.Username,
            student.Balance
        ));
    }

    [Authorize(Roles = "1")]
    [HttpDelete("slots/{slotId}/students/{studentId}")]
    public async Task<IActionResult> RemoveStudentFromSlot(int slotId, int studentId)
    {
        var enrollment = await _context.ScheduleEnrollments
            .FirstOrDefaultAsync(e => e.ScheduleSlotId == slotId && e.StudentId == studentId);

        if (enrollment == null)
            return NotFound(new { message = "Öğrenci bu ders saatinde bulunamadı." });

        _context.ScheduleEnrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Öğrenci dersten çıkarıldı." });
    }
}
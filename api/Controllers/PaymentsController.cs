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

public class PaymentsController(AppDbContext _context, IHubContext<AttendanceHub> _hubContext) : ControllerBase
{

    [Authorize(Roles = "1")]
    [HttpPost("add")]
    public async Task<IActionResult> AddPayment([FromBody] AddPaymentRequest request)
    {
        var currentInstructorId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.StudentId && u.Role == UserRole.Student);
        
        if (student == null)
            return NotFound(new { Message = "Öğrenci bulunamadı." });

        student.Balance += request.AddedSessions;

        var paymentLog = new PaymentLog
        {
            StudentId = student.Id,
            InstructorId = currentInstructorId,
            AddedSessions = request.AddedSessions,
            Amount = request.Amount,
            PaymentDate = DateTime.UtcNow
        };

        _context.PaymentLogs.Add(paymentLog);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveAttendanceUpdate", student.Id, student.Balance);

        return Ok(new 
        { 
            Message = $"{request.AddedSessions} ders başarıyla eklendi.", 
            NewBalance = student.Balance 
        });
    }
}
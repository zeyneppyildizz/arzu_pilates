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
public class UsersController(AppDbContext _context, IHubContext<AttendanceHub> _hubContext) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("create-student")]
    public async Task<IActionResult> CreateStudent([FromBody] CreateUserRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username.ToLower() == request.Username.ToLower()))
        {
            return BadRequest(new { message = "Bu kullanıcı adı zaten kullanılıyor. Lütfen farklı bir kullanıcı adı belirleyin." });
        }

        var student = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Student,
            Balance = 0
        };

        _context.Users.Add(student);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveNewStudent");

        return Ok(new { message = "Öğrenci başarıyla oluşturuldu.", StudentId = student.Id });
    }

    [Authorize(Roles = "1")]
    [HttpGet("students")]
    public async Task<IActionResult> GetStudents()
    {
        var students = await _context.Users
            .Where(u => u.Role == UserRole.Student)
            .OrderBy(u => u.FirstName)
            .Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.Username,
                u.Balance,
                HasAnyPayment = u.Payments.Any()
            })
            .ToListAsync();

        return Ok(students);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        var user = await _context.Users
            .Where(u => u.Id == currentUserId)
            .Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.Username,
                u.Balance,
                HasAnyPayment = u.Payments.Any()
            })
            .FirstOrDefaultAsync();
            
        if (user == null)
            return Unauthorized(new { message = "Hesabınız aktif değil veya silinmiş." });

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Unauthorized(new { message = "Kullanıcı bulunamadı veya şifre hatalı." });
        }

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, ((int)user.Role).ToString())
        };

        var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? ""));

        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.Role,
            Token = tokenString
        });
    }


    [Authorize(Roles = "1")]
    [HttpPost("add-student")]
    public async Task<IActionResult> AddStudent([FromBody] AddStudentRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username.ToLower() == request.Username.ToLower()))
        {
            return BadRequest(new { message = "Bu kullanıcı adı zaten kullanılıyor." });
        }

        var newStudent = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Student,
            Balance = request.Balance
        };

        _context.Users.Add(newStudent);
        await _context.SaveChangesAsync();

        if (request.Balance > 0)
        {
            var initialPaymentLog = new PaymentLog
            {
                StudentId = newStudent.Id,
                AddedSessions = request.Balance,
                Amount = request.Amount,
                PaymentDate = DateTime.UtcNow
            };

            _context.PaymentLogs.Add(initialPaymentLog);
            await _context.SaveChangesAsync();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveNewStudent");

        return Ok(new { message = "Öğrenci başarıyla eklendi." });
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetStudentHistory(int id)
    {
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var currentUserRole = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "0");

        if (currentUserRole == 0 && currentUserId != id)
        {
            return Forbid();
        }

        var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Student);

        if (student == null)
            return NotFound(new { message = "Öğrenci bulunamadı." });

        var attendances = await _context.AttendanceLogs
            .Where(a => a.StudentId == id)
            .OrderByDescending(a => a.AttendedAt)
            .Select(a => new AttendanceDto(a.Id, a.AttendedAt, a.BalanceAfterAttendance))
            .ToListAsync();

        var payments = await _context.PaymentLogs
            .Where(p => p.StudentId == id)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new PaymentDto(p.Id, p.PaymentDate, p.AddedSessions, p.Amount))
            .ToListAsync();

        return Ok(new StudentHistoryResponse(attendances, payments));
    }

    [Authorize(Roles = "1")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Student);

        if (student == null)
            return NotFound(new { Message = "Öğrenci bulunamadı." });

        student.IsActive = false;
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("UserDeleted", id);
        return Ok(new { Message = "Öğrenci başarıyla silindi." });
    }

    [HttpGet("{id}/measurements")]
    public async Task<IActionResult> GetMeasurements(int id)
    {
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var currentUserRole = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "0");

        if (currentUserRole == 0 && currentUserId != id)
        {
            return Forbid();
        }

        var measurement = await _context.StudentMeasurements.FirstOrDefaultAsync(m => m.StudentId == id);

        if (measurement == null)
            return Ok(new MeasurementDto(null, null, null, null, null, null, null, null, null));

        return Ok(new MeasurementDto(
            measurement.Height, measurement.Weight, measurement.Waist, measurement.Abdomen, measurement.Hips,
            measurement.RightLeg, measurement.LeftLeg, measurement.RightArm, measurement.LeftArm
        ));
    }

    [HttpPost("{id}/measurements")]
    public async Task<IActionResult> UpdateMeasurements(int id, [FromBody] MeasurementDto dto)
    {
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var currentUserRole = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "0");

        if (currentUserRole == 0 && currentUserId != id)
        {
            return Forbid();
        }

        var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Student);
        if (student == null)
            return NotFound(new { message = "Öğrenci bulunamadı." });

        var measurement = await _context.StudentMeasurements.FirstOrDefaultAsync(m => m.StudentId == id);

        if (measurement == null)
        {
            measurement = new StudentMeasurement { StudentId = id };
            _context.StudentMeasurements.Add(measurement);
        }

        measurement.Height = dto.Height;
        measurement.Weight = dto.Weight;
        measurement.Waist = dto.Waist;
        measurement.Abdomen = dto.Abdomen;
        measurement.Hips = dto.Hips;
        measurement.RightLeg = dto.RightLeg;
        measurement.LeftLeg = dto.LeftLeg;
        measurement.RightArm = dto.RightArm;
        measurement.LeftArm = dto.LeftArm;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Ölçüler başarıyla güncellendi." });
    }
}
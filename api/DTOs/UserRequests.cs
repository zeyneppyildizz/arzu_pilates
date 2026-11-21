using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public record LoginRequest(
    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    string Username,

    [Required(ErrorMessage = "Şifre zorunludur.")]
    string Password
);

public record CreateUserRequest(
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Ad 2 ile 50 karakter arasında olmalıdır.")]
    string FirstName,

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Soyad 2 ile 50 karakter arasında olmalıdır.")]
    string LastName,

    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3 ile 30 karakter arasında olmalıdır.")]
    string Username,

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(3, ErrorMessage = "Şifre en az 3 karakter olmalıdır.")]
    string Password
);

public record AddStudentRequest(
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50, MinimumLength = 2)]
    string FirstName,

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50, MinimumLength = 2)]
    string LastName,

    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    [StringLength(30, MinimumLength = 3)]
    string Username,

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(3)]
    string Password,

    [Range(0, 500, ErrorMessage = "Başlangıç bakiyesi negatif olamaz.")]
    int Balance,

    [Range(0, double.MaxValue, ErrorMessage = "Tutar negatif olamaz.")]
    decimal Amount
);

public record ChangePasswordRequest(
    string? CurrentPassword,

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [MinLength(3, ErrorMessage = "Yeni şifre en az 3 karakter olmalıdır.")]
    string NewPassword
);

public record UpdateProfileRequest(
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Ad 2 ile 50 karakter arasında olmalıdır.")]
    string FirstName,

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Soyad 2 ile 50 karakter arasında olmalıdır.")]
    string LastName,

    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3 ile 30 karakter arasında olmalıdır.")]
    string Username
);
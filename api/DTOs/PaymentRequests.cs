using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public record AddPaymentRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir öğrenci seçilmelidir.")]
    int StudentId,

    [Range(1, 500, ErrorMessage = "Eklenen ders sayısı en az 1 olmalıdır.")]
    int AddedSessions,

    [Range(0, double.MaxValue, ErrorMessage = "Ödeme tutarı negatif olamaz.")]
    decimal Amount
);
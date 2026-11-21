using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public record MeasurementDto(
    [Range(50, 250, ErrorMessage = "Boy 50 ile 250 cm arasında olmalıdır.")]
    double? Height,

    [Range(20, 300, ErrorMessage = "Kilo 20 ile 300 kg arasında olmalıdır.")]
    double? Weight,

    [Range(30, 250, ErrorMessage = "Bel 30 ile 250 cm arasında olmalıdır.")]
    double? Waist,

    [Range(30, 250, ErrorMessage = "Karın 30 ile 250 cm arasında olmalıdır.")]
    double? Abdomen,

    [Range(30, 250, ErrorMessage = "Kalça 30 ile 250 cm arasında olmalıdır.")]
    double? Hips,

    [Range(20, 150, ErrorMessage = "Sağ bacak 20 ile 150 cm arasında olmalıdır.")]
    double? RightLeg,

    [Range(20, 150, ErrorMessage = "Sol bacak 20 ile 150 cm arasında olmalıdır.")]
    double? LeftLeg,

    [Range(10, 100, ErrorMessage = "Sağ kol 10 ile 100 cm arasında olmalıdır.")]
    double? RightArm,

    [Range(10, 100, ErrorMessage = "Sol kol 10 ile 100 cm arasında olmalıdır.")]
    double? LeftArm
);
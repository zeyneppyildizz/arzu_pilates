using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public record MeasurementDto(
    [Range(0, 300, ErrorMessage = "Boy 0 ile 300 cm arasında olmalıdır.")]
    double? Height,

    [Range(0, 500, ErrorMessage = "Kilo 0 ile 500 kg arasında olmalıdır.")]
    double? Weight,

    [Range(0, 300)] double? Waist,
    [Range(0, 300)] double? Abdomen,
    [Range(0, 300)] double? Hips,
    [Range(0, 200)] double? RightLeg,
    [Range(0, 200)] double? LeftLeg,
    [Range(0, 200)] double? RightArm,
    [Range(0, 200)] double? LeftArm
);
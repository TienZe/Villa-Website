using System;
using System.ComponentModel.DataAnnotations;

namespace VillaAPI.Models.Dto;

public class VillaCreateDTO
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(30)]
    public string Name { get; set; }

    public int Occupancy { get; set; }
    public int Sqft { get; set; }
    public string? Details { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rate is required")]
    public double? Rate { get; set; }

    public string? ImageUrl { get; set; } = string.Empty;
    public string? Amenity { get; set; } = string.Empty;
}
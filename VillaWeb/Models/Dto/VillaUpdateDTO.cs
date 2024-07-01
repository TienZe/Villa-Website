using System;
using System.ComponentModel.DataAnnotations;

namespace VillaWeb.Models.Dto;

public class VillaUpdateDTO
{
    public int Id { get; set; }
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int Occupancy { get; set; }

    [Required]
    public int Sqft { get; set; }
    public string? Details { get; set; } = string.Empty;

    [Required]
    public double Rate { get; set; }
    public string? ImageUrl { get; set; }
    public string? Amenity { get; set; } = string.Empty;
    public IFormFile? NewImage { get; set; }
}
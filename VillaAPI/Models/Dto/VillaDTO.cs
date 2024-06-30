using System;
using System.ComponentModel.DataAnnotations;

namespace VillaAPI.Models.Dto;

public class VillaDTO
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    public int Occupancy { get; set; }
    public int Sqft { get; set; }
    public string Details { get; set; } = string.Empty;

    [Required]
    public double Rate { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageLocalPath { get; set; }
    public string Amenity { get; set; } = string.Empty;
}
﻿using System;
using System.ComponentModel.DataAnnotations;

namespace VillaAPI.Models.Dto;

public class VillaUpdateDTO
{
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int Occupancy { get; set; }

    [Required]
    public int Sqft { get; set; }
    public string? Details { get; set; } = string.Empty;

    [Required]
    public double Rate { get; set; }

    public IFormFile? NewImage { get; set; }
    
    public string? Amenity { get; set; }
}
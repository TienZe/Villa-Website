﻿using System;
using System.ComponentModel.DataAnnotations;

namespace VillaWeb.Models.Dto;

public class VillaCreateDTO
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(30)]
    public string Name { get; set; }

    public int Occupancy { get; set; }
    public int Sqft { get; set; }
    public string? Details { get; set; }

    [Required(ErrorMessage = "Rate is required")]
    public double? Rate { get; set; }

    public IFormFile? Image { get; set; }
    public string? Amenity { get; set; }
}
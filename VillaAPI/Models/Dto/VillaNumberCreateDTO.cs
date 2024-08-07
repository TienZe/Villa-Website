﻿using System;
using System.ComponentModel.DataAnnotations;

namespace VillaAPI.Models.Dto;

public class VillaNumberCreateDTO
{
    [Required]
    public int VillaNo { get; set; }
    public string? SpecialDetails { get; set; }
    [Required]
    public int VillaID { get; set; }
}
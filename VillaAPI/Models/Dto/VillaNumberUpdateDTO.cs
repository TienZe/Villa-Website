using System;
using System.ComponentModel.DataAnnotations;

namespace VillaAPI.Models.Dto;

public class VillaNumberDTO
{
    [Required]
    public int VillaNo { get; set; }
    public string SpecialDetails { get; set; }
}
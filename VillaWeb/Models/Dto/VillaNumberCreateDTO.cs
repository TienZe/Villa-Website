using System;
using System.ComponentModel.DataAnnotations;

namespace VillaWeb.Models.Dto;

public class VillaNumberCreateDTO
{
    [Required]
    public int VillaNo { get; set; }
    public string? SpecialDetails { get; set; }
    [Required(ErrorMessage = "Please select a villa")]
    public int? VillaID { get; set; }
}
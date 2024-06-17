using System;
using System.ComponentModel.DataAnnotations;

namespace VillaAPI.Models.Dto;

public class VillaNumberDTO
{
    public int VillaNo { get; set; }
    public string SpecialDetails { get; set; }
    public int VillaID { get; set; }
    public VillaDTO Villa { get; set; }
}
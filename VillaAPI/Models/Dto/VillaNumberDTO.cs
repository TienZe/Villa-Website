using System;
using System.ComponentModel.DataAnnotations;

namespace VillaAPI
{
    public class VillaNumberDTO
    {
        [Required]
        public int VillaNo { get; set; }
        public string SpecialDetails { get; set; }
    }
}
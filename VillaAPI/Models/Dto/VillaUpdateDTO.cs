using System;
using System.ComponentModel.DataAnnotations;

namespace VillaAPI
{
    public class VillaUpdateDTO
    {
        [MaxLength(30)]
        public String Name { get; set; } = String.Empty;

        [Required]
        public int Occupancy { get; set; }

        [Required]
        public int Sqft { get; set; }
        public String Details { get; set; } = String.Empty;
        
        [Required]
        public double Rate { get; set; }
        [Required]
        public String ImageUrl { get; set; } = String.Empty;
        public String Amenity { get; set; } = String.Empty;
    }
}
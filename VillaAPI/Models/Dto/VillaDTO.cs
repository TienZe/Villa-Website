using System;
using System.ComponentModel.DataAnnotations;

namespace VillaAPI
{
    public class VillaDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public String Name { get; set; } = String.Empty;

        public int Occupancy { get; set; }
        public int Sqft { get; set; }
        public String Details { get; set; } = String.Empty;
        
        [Required]
        public double Rate { get; set; }
        
        public String ImageUrl { get; set; } = String.Empty;
        public String Amenity { get; set; } = String.Empty;
    }
}
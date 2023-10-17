﻿using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class PointOfInterestUpdateDto
    {
        [Required(ErrorMessage = "You must provide a name.")]
        [MaxLength(50)]
        public string Name {get; set; } = string.Empty;
        [MaxLength(200)]
        public string? Description {get; set; }
    }
}

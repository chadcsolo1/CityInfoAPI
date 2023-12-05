using CityInfo.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(200)]
        public string? Description { get; set; }
        public ICollection<PointOfInterest>  PointsOfInterest  { get; set; }
            = new List<PointOfInterest> ();
        
        //We always want a City to have a name and we are conveying that by 
        //creating this constructor giving it a name parameter
        public City (string name)
        {
            Name = name;
        }
    }
}

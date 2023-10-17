namespace CityInfo.API.Models
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        //When using a collection like below, alwasy initialize an instance so to 
        //avoid null reference exceptions.
        public ICollection<PointOfInterestDto> PointOfInterests { get; set;}
            = new List<PointOfInterestDto>();
    }
}   

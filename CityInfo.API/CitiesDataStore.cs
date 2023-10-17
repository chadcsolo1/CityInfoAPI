using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; }
        //This keeps the data store as a singleton, while using the static keyword
        //So while we are in the same application session, we will continue to work on the
        //same CitiesDataStore
        public static CitiesDataStore Current { get; set; } = new CitiesDataStore();
        public CitiesDataStore()
        {
           Cities = new List<CityDto>()
           {
                new CityDto()
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "The one with the big park.",
                    PointOfInterests = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Empire State Building",
                            Description = "Very Tall Building"
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Statue of Liberty",
                            Description = "Very Tall Statue"
                        }
                    }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "The one with the cathedral that was never really finished.",
                    PointOfInterests = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Cathedral",
                            Description = "Famous unfinished cathedral"
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Kevin Dockx",
                            Description = "Microsoft MVP"
                        }
                    }
                },
                new CityDto()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one with the big tower.",
                    PointOfInterests = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Eiffel Tower",
                            Description = "Famous Tower in Paris"
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "The Louvre",
                            Description = "The Worlds Largest Museum"
                        }
                    }
                }
           };
        }
    }
}

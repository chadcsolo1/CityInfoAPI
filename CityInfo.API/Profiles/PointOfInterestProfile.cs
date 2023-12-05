using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class PointOfInterestProfile : Profile
    {
        public PointOfInterestProfile() 
        {
             CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();

            CreateMap<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>();

            //Pay attention to the following 2 mappings.
            //1st the updatedto is the source and the entity is the destination
            //2nd the entity is the source and the updatedto is the destination.
            CreateMap<Models.PointOfInterestUpdateDto, Entities.PointOfInterest>();

            CreateMap<Entities.PointOfInterest, Models.PointOfInterestUpdateDto>();
        }
    }
}

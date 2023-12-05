using CityInfo.API.Entities;
using CityInfo.API.Services;

namespace CityInfo.API.Data
{
    public interface ICityInfoRepository
    {
        //We can return either an IEnumerable or an IQueryable from our GET methods
        //an IQueryable allows us to build on the IQueryable, for instance we can add 
        //a where clause or an orderby clause before the query is executed.
        //IQueryable<City> GetCities();
        Task<IEnumerable<City>> GetCitiesAync();

         Task<(IEnumerable<City>, PaginationMetaData)> GetCityByNameAsync(string? name, string? searchQuery, int pageNumber, int pageSize);

        Task<City?> GetCityAync(int cityId, bool includePointsOfInterest);

        Task<bool> CityExistAsync(int cityId);

        Task<IEnumerable<PointOfInterest>> GetPointOfInterestAync(int cityId);

        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);

        //The only reason we make this Adding method async is
        //because we call an async method inside of AddPointOfInterestForCityAsync
        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

        //DeletePointOfInterest is not async
        //Deleting just like adding is an in memory operation
        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        Task<bool> CityNameMatchesCityId(string? cityName, int cityId);

        Task<bool>  SaveChangesAsync();
        
    }
}

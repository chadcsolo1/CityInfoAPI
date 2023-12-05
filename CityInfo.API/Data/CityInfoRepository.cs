using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using CityInfo.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CityInfo.API.Data
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<(IEnumerable<City>, PaginationMetaData)> GetCityByNameAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            

            //By using an IQueryable<City> we are implementing Deferred Execution
            //because since we made collection of type IQueryable<City>, the query will 
            //not be executed on the DB until iterate over collection. 
            //In this case that happens when we call .ToList(); in the return statement.
            var collection = _context.Cities as IQueryable<City>;

            //Filtering
            if(!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            //Searching - Account for search against the name and description of the city
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery) 
                || (a.Description != null && a.Description.Contains(searchQuery)));
            }

            //We get totalItemCount by calling .CountAsync() on the IQueriable collection
            //which returns the number of elements in the input sequence.
            var totalItemCount = await collection.CountAsync();

            //Constructing the metadata
            var paginationMetadata = new PaginationMetaData(totalItemCount, pageSize, pageNumber);


            //Our Paging fxnality is found in the .Skip() & .Take() methods below.
            //.Skip() will skip a specified number of elements in a collection an return the remaining.
            //So if a user enters wants page 5 and the pageSize is 10 then we will skip the first 40 elements of the collection
            //which would be the first 4 pages.
            //.Take() will take the given number of elements and return them
            var collectionToReturn = await collection.OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<City?> GetCityAync(int cityId, bool includePointsOfInterest)
        {
            if(includePointsOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointOfInterestAync(int cityId)
        {
            return await _context.PointsOfInterest.Where(c => c.Id == cityId).ToListAsync();            
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointsOfInterest.Where(c => c.Id == cityId && c.Id == pointOfInterestId)
                .FirstOrDefaultAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            //Find City & if city is not null add POI
            var city = await GetCityAync(cityId, false);

            if(city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            if(pointOfInterest != null)
            {
                _context.PointsOfInterest.Remove(pointOfInterest);
            }else
            {
                throw new ArgumentNullException(nameof(pointOfInterest));
            }

        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);

        }

        public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }
    }
}

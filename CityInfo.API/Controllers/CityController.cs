using AutoMapper;
using CityInfo.API.Data;
using CityInfo.API.DbContexts;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    [Authorize]
    public class CityController : ControllerBase
    {
        
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public CityController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        //The string? name parameter could also be written like this GetCities([FromQuery] string? name)
        //and if your variable does not have the same name as the key (key = name) in the query string we can use the 
        //the name property like so [FromQuery(Name = "filteronname")]
        [HttpGet]
        public async Task<ActionResult<(IEnumerable<CityWithoutPointOfInterestDto>, PaginationMetaData)>> GetCities(string? name, string? searchQuery,
            int pageNumber = 1, int pageSize = 10)
        {
            if(pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }

            var (cityEntity, paginationMetadata) = await _cityInfoRepository
                .GetCityByNameAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntity));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCity(int id, bool includePointOfInterest = false)
        {

            var city = await _cityInfoRepository.GetCityAync(id, includePointOfInterest);

            if(city == null)
            {
                return NotFound();
            }

            if (includePointOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithoutPointOfInterestDto>(city));
        }
    }
}

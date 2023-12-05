using AutoMapper;
using CityInfo.API.Data;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [Authorize(Policy = "MustBeFromManila")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _localMailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, 
            IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localMailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>>  GetPointsOfInterest(int cityId)
        {

            var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

            if (!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
            {
                return Forbid();
            }

            if(!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"City with Id {cityId} was not found when accessing points of interest.");

                return NotFound();
            }

            var pointsOfInterestForCity = await _cityInfoRepository.GetPointOfInterestAync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            

            //check if city exist
            if(!await _cityInfoRepository.CityExistAsync(cityId))
            {
                
                return NotFound();
            }
            
            //get POI
            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            //check if POI exist
            if(pointOfInterest == null)           
            {
                return NotFound();
            }

            //return mapped results
            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId,
            PointOfInterestForCreationDto pointOfInterest)
        {
           if(! await _cityInfoRepository.CityExistAsync(cityId))
           {
                return NotFound();
           }
           
           var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                },
                createdPointOfInterestToReturn);
            
        }

        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestUpdateDto pointOfInterestUpdate)
        {
            //Find the city
            
            if (! await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            // Use GetPointOfInterestForCityAsync
            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }
            
            //_mapper.Map(source, destination); source of updated information = pointOfInterestUpdate;
            _mapper.Map(pointOfInterestUpdate, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            //Return no Content
            return NoContent();
        }

        //Important: the patch document is on the DTO and not directly on the entity because we do not want to expose
        //entity implementation info to the outer facing layer.

        //we will have to find the entity first then map it to a DTO before applying a patch document

        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PariallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {

            if (! await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }
         
            var pointOfInterestToPatch = _mapper.Map<PointOfInterestUpdateDto>(pointOfInterestEntity);

            //Apply Patch to POItoPatch and pass Model State
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            //Checking the ModelState of the JSON Patch Document
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            
            await _cityInfoRepository.SaveChangesAsync();

            //return NoContent
            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            //Find city
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            //Find POI entity
            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();

        }
    }
}

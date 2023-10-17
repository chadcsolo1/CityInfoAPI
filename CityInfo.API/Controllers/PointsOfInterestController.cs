using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>>  GetPointsOfInterest(int cityId)
        {
            try
            {

                var city = CitiesDataStore.Current.Cities
                    .FirstOrDefault(x => x.Id == cityId);

                if(city == null)
                {
                    _logger.LogInformation($"City with id {cityId} was not found when accessing points of interest.");
                    return NotFound();
                }

                return Ok(city.PointOfInterests);
            }catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.",
                    ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            //Get the city first
            var city = CitiesDataStore.Current.Cities
                .FirstOrDefault(x => x.Id == cityId);

            if(city == null)
            {
                return NotFound();
            }

            //Get the City's points of interest
            var pointOfInterest = city.PointOfInterests
                .FirstOrDefault(x => x.Id == pointOfInterestId);

            if(pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId,
            PointOfInterestForCreationDto pointOfInterest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Retrieve city base on param ID
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

            //Does City exist
            if (city == null)
            {
                return NotFound();
            }

            //Demo Purpose - to be improved
            //Find maxpoint of interest and add 1
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => 
                c.PointOfInterests).Max(x => x.Id);

            //we will map out the to be newely created point of interest
            //by creating a new pointOfInterest object
            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            //add the final point of interest to the retrieved city
            city.PointOfInterests.Add(finalPointOfInterest);

            //
            return CreatedAtRoute("GetPointOfInterest", 
                new
                {
                    cityId = cityId,
                    pointOfInterestId = finalPointOfInterest.Id
                    
                },
                finalPointOfInterest
                );
            
        }

        [HttpPut("{pointofinterestid}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestUpdateDto pointOfInterestUpdate)
        {
            //Find the city
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //Find the pointofinterest
            var pointOfInterestFromStore = city.PointOfInterests.FirstOrDefault(
                x => x.Id == pointOfInterestId);


            //Map POI name and description
            pointOfInterestFromStore.Name = pointOfInterestUpdate.Name;
            pointOfInterestFromStore.Description = pointOfInterestUpdate.Description;

            //Return no Content
            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public ActionResult PariallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            //Find City
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //Find POI
            var POIFromStore = city.PointOfInterests.FirstOrDefault(x => x.Id == pointOfInterestId);

            //New POIUpdateDto with desired properties to change
            var POItoPatch = new PointOfInterestUpdateDto()
            {
                Name = POIFromStore.Name,
                Description = POIFromStore.Description,
            };

            //Apply Patch to POItoPatch and pass Model State
            patchDocument.ApplyTo(POItoPatch, ModelState);

            //Checking the ModelState of the JSON Patch Document
            if(!ModelState.IsValid) 
            {                
                return BadRequest();
            }

            if (!TryValidateModel(POItoPatch))
            {
                return BadRequest(ModelState);
            }

            //Map Properties
            POIFromStore.Name = POItoPatch.Name;
            POIFromStore.Description = POItoPatch.Description;

            //return NoContent
            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            //Find city
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

            if(city == null)
            {
                return NotFound();
            }

            //Find POI
            var POIFromStore = city.PointOfInterests.FirstOrDefault(x => x.Id == pointOfInterestId);

            if(POIFromStore == null)
            {
                return NotFound();
            }

            city.PointOfInterests.Remove(POIFromStore);
            return NoContent();

        }
    }
}

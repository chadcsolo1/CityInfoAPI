using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CityController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id) 
        {
            
            var myCity = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);

            if (myCity == null)
            {
                return NotFound();
            }
            
            return Ok(myCity);
        }
    }
}

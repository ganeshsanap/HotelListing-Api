using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers.v2
{
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCountries()
        {
            return Ok("Response from Country V2");
        }
    }
}

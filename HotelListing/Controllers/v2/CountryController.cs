using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

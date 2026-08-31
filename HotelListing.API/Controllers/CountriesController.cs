using HotelListing.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        ICountryService _countryService;
        public CountriesController(ICountryService countryService)
        {
            countryService = _countryService;
        }

    }
}
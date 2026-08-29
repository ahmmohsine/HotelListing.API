using HotelListing.API.Data;
using HotelListing.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepository _repository;

        public CountriesController(ICountryRepository repository)
        {
            _repository = repository;
        }

        // GET api/countries
        [HttpGet]
        public ActionResult<IEnumerable<Country>> Get()
        {
            return Ok(_repository.GetAll());
        }

        // GET api/countries/5
        [HttpGet("{id}", Name = "GetCountryById")]
        public ActionResult<Country> Get(int id)
        {
            var country = _repository.GetById(id);
            if (country == null)
                return NotFound();
            return Ok(country);
        }

        // POST api/countries
        [HttpPost]
        public ActionResult<Country> Post([FromBody] Country newCountry)
        {
            try
            {
                _repository.Add(newCountry);
                return CreatedAtRoute("GetCountryById", new { id = newCountry.Id }, newCountry);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/countries/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Country updatedCountry)
        {
            try
            {
                _repository.Update(id, updatedCountry);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE api/countries/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                _repository.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
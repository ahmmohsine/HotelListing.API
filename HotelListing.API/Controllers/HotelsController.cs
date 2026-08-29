using HotelListing.API.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private static readonly List<Hotel> hotels = Seed.GenerateHotels();
        // private static readonly List<Hotel> hotels = new List<Hotel>() { new Hotel() { Id = 1, Name = "test", Address = "Adresse", Rating = 4.5 } };
        [HttpGet]
        public ActionResult<IEnumerable<Hotel>> Get()
        {
            return Ok(hotels);
        }

        // GET api/<HotelsController>/5
        [HttpGet("{id}")]
        public ActionResult<Hotel> Get(int id)
        {
            var h = hotels.FirstOrDefault(h => h.Id == id);
            if (h == null)
                return NotFound();
            return Ok(h);
        }

        // POST api/<HotelsController>
        [HttpPost]
        public ActionResult<Hotel> Post([FromBody] Hotel newHotel)
        {
            if (hotels.Any(hotel => hotel.Id == newHotel.Id))
            {
                return BadRequest("Hotelwith this Id already exists");
            }
            hotels.Add(newHotel);
            return CreatedAtRoute("GetHotelById", new { id = newHotel.Id }, newHotel);
        }

        // PUT api/<HotelsController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Hotel updatedHotel)
        {
            var existingHotel = hotels.FirstOrDefault(hotel => hotel.Id == id);
            if (existingHotel == null)
            {
                return NotFound();
            }
            existingHotel.Address = updatedHotel.Address;
            existingHotel.Name = updatedHotel.Name;
            existingHotel.Rating = updatedHotel.Rating;
            return NoContent();
        }

        // DELETE api/<HotelsController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var existingHotel = hotels.FirstOrDefault(hotel => hotel.Id == id);
            if (existingHotel == null)
            {
                return NotFound(new { message = "Hotel not found!" });
            }
            hotels.Remove(existingHotel);
            return NoContent();
        }
    }
}

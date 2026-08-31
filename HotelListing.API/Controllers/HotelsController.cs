using HotelListing.API.DTOs;
using HotelListing.API.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;


        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HotelReadOnlyDto>))]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var hotels = await _hotelService.GetAllAsync(ct);
            return Ok(hotels);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HotelReadOnlyDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var hotel = await _hotelService.GetByIdAsync(id, ct);

            if (hotel is null)
            {
                return NotFound(new { Message = $"L'hôtel avec l'ID {id} n'a pas été trouvé." });
            }

            return Ok(hotel);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(HotelReadOnlyDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateHotelDto dto, CancellationToken ct)
        {
            var createdHotel = await _hotelService.CreateAsync(dto, ct);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdHotel.Id },
                createdHotel
            );
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateHotelDto dto, CancellationToken ct)
        {
            var updated = await _hotelService.UpdateAsync(id, dto, ct);

            if (!updated)
            {
                return NotFound(new { Message = $"Impossible de mettre à jour. L'hôtel avec l'ID {id} n'existe pas." });
            }

            return NoContent();
        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            var deleted = await _hotelService.DeleteAsync(id, ct);

            if (!deleted)
            {
                return NotFound(new { Message = $"Impossible de supprimer. L'hôtel avec l'ID {id} n'existe pas." });
            }

            return NoContent();
        }

    }
}

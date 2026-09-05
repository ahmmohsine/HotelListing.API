using HotelListing.API.DTOs.Country;
using HotelListing.API.DTOs.Hotel;
using HotelListing.API.Results;
using HotelListing.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers;

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
        var result = await _hotelService.GetAllAsync(ct);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Value.FirstOrDefault()?.Id }, result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HotelReadOnlyDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var result = await _hotelService.GetByIdAsync(id, ct);
        return result.ToActionResult();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(HotelReadOnlyDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateHotelDto dto, CancellationToken ct)
    {
        var result = await _hotelService.CreateAsync(dto, ct);

        return result.ToActionResult();
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateHotelDto dto, CancellationToken ct)
    {
        var result = await _hotelService.UpdateAsync(id, dto, ct);
        return result.ToActionResult();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var result = await _hotelService.DeleteAsync(id, ct);
        return result.ToActionResult();
    }
}
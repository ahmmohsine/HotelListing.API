using HotelListing.API.DTOs;
using HotelListing.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ICountryService _countryService;

    public CountriesController(ICountryService countryService)
    {
        _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CountryReadOnlyDto>))]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var countries = await _countryService.GetAllAsync(ct);
        return Ok(countries);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CountryReadOnlyDto))]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var country = await _countryService.GetByIdAsync(id, ct);
        if (country == null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Ressource introuvable",
                detail: $"Le pays avec l'ID {id} n'existe pas."
            );
        }

        return Ok(country);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CountryReadOnlyDto))]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDto dto, CancellationToken ct)
    {
        var country = await _countryService.CreateCountryAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = country.Id }, country);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCountry([FromRoute] int id, [FromBody] UpdateCountryDto dto, CancellationToken ct)
    {
        var updated = await _countryService.UpdateAsync(id, dto, ct);
        if (!updated)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Ressource introuvable",
                detail: $"Impossible de mettre à jour. Le pays avec l'ID {id} n'existe pas."
            );
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCountry([FromRoute] int id, CancellationToken ct)
    {
        var deleted = await _countryService.DeleteCountryAsync(id, ct);
        if (!deleted)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Ressource introuvable",
                detail: $"Impossible de supprimer. Le pays avec l'ID {id} n'existe pas."
            );
        }

        return NoContent();
    }
}
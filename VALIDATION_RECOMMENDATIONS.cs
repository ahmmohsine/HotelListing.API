// RECOMMENDED: Add validation to models
// File: HotelListing.API/Data/Country.cs

using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Data
{
    /// <summary>
    /// RECOMMENDED VERSION with input validation.
    /// This version includes data annotations to prevent invalid data submission.
    /// </summary>
    public class CountrySecure
    {
        [Range(1, int.MaxValue, ErrorMessage = "Country ID must be a positive integer")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Country name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Country name must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s'-]*$", ErrorMessage = "Country name can only contain letters, spaces, hyphens, and apostrophes")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Country code is required")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Country code must be exactly 2 characters")]
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "Country code must be exactly 2 uppercase letters (ISO 3166-1 alpha-2)")]
        public string Code { get; set; }
    }
}

// ============================================================================
// RECOMMENDED: Hotel.cs with validation
// ============================================================================

using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Data
{
    public class HotelSecure
    {
        [Range(1, int.MaxValue, ErrorMessage = "Hotel ID must be a positive integer")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hotel name is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Hotel name must be between 2 and 150 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Hotel address is required")]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Hotel address must be between 5 and 250 characters")]
        public string Address { get; set; }

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public double Rating { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Country ID must be a positive integer")]
        public int CountryId { get; set; } // Foreign key

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? PricePerNight { get; set; }

        [EmailAddress(ErrorMessage = "Contact email must be a valid email address")]
        public string ContactEmail { get; set; }

        [Phone(ErrorMessage = "Contact phone must be a valid phone number")]
        public string ContactPhone { get; set; }
    }
}

// ============================================================================
// RECOMMENDED: Add FluentValidation for complex validation rules
// ============================================================================

/*
Install-Package FluentValidation
Install-Package FluentValidation.AspNetCore

In ServiceCollectionExtensions:

services.AddFluentValidationAutoValidation();
services.AddValidatorsFromAssemblyContaining<Program>();

Create Validators/CountryValidator.cs:
*/

using FluentValidation;

namespace HotelListing.API.Validators
{
    public class CountryValidator : AbstractValidator<Country>
    {
        public CountryValidator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Country ID must be positive");

            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Country name is required")
                .Length(2, 100)
                .WithMessage("Country name must be between 2 and 100 characters")
                .Matches(@"^[a-zA-Z\s'-]*$")
                .WithMessage("Country name contains invalid characters");

            RuleFor(c => c.Code)
                .NotEmpty()
                .WithMessage("Country code is required")
                .Length(2)
                .WithMessage("Country code must be exactly 2 characters")
                .Matches(@"^[A-Z]{2}$")
                .WithMessage("Country code must be 2 uppercase letters (ISO 3166-1)");

            // Business rule validation
            RuleFor(c => c)
                .Custom((country, context) =>
                {
                    // Example: Check if country code format matches known format
                    var validCodes = new[] { "US", "CA", "MX", "UK", "FR", "DE", "IT", "ES" };
                    if (!validCodes.Contains(country.Code))
                    {
                        context.AddFailure("Code", "Country code is not in our supported list");
                    }
                });
        }
    }
}

// ============================================================================
// RECOMMENDED: CustomValidationResult for standardized error responses
// ============================================================================

namespace HotelListing.API.Models
{
    public class ValidationErrorResponse
    {
        public string Type { get; set; } = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        public string Title { get; set; } = "One or more validation errors occurred";
        public int Status { get; set; } = 400;
        public Dictionary<string, string[]> Errors { get; set; } = new();
    }
}

/*
In Program.cs, add after AddControllers():

services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(ms => ms.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var response = new ValidationErrorResponse
        {
            Errors = errors
        };

        return new BadRequestObjectResult(response);
    };
});
*/

// ============================================================================
// RECOMMENDED: Example Controller with proper validation
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace HotelListing.API.Controllers.Recommended
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesSecureController : ControllerBase
    {
        private readonly ICountryRepository _repository;
        private readonly IValidator<Country> _validator;
        private readonly ILogger<CountriesSecureController> _logger;

        public CountriesSecureController(
            ICountryRepository repository,
            IValidator<Country> validator,
            ILogger<CountriesSecureController> logger)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
        }

        // GET api/countries
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<Country>> Get()
        {
            try
            {
                return Ok(_repository.GetAll());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving countries");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Failed to retrieve countries. Please try again." });
            }
        }

        // GET api/countries/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Country> Get(int id)
        {
            // Input validation
            if (id <= 0)
            {
                _logger.LogWarning("Invalid country ID requested: {Id}", id);
                return BadRequest(new { error = "Country ID must be a positive integer" });
            }

            var country = _repository.GetById(id);
            if (country == null)
            {
                _logger.LogInformation("Country not found: {Id}", id);
                return NotFound();
            }

            return Ok(country);
        }

        // POST api/countries
        [HttpPost]
        [Authorize(Roles = "Admin")]  // Requires authorization
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Country>> Post([FromBody] Country newCountry)
        {
            if (newCountry == null)
            {
                return BadRequest(new { error = "Country data is required" });
            }

            // Validate using FluentValidation
            var validationResult = await _validator.ValidateAsync(newCountry);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                _logger.LogWarning("Validation failed for new country");
                return BadRequest(new { errors });
            }

            try
            {
                _repository.Add(newCountry);
                _logger.LogInformation("Country created: {CountryId}", newCountry.Id);
                return CreatedAtRoute("GetCountryById", new { id = newCountry.Id }, newCountry);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Duplicate country ID: {CountryId}", newCountry.Id);
                return BadRequest(new { error = "A country with this ID already exists" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating country");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Failed to create country. Please try again." });
            }
        }

        // PUT api/countries/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(int id, [FromBody] Country updatedCountry)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Country ID must be a positive integer" });
            }

            if (updatedCountry == null)
            {
                return BadRequest(new { error = "Country data is required" });
            }

            // Validate
            var validationResult = await _validator.ValidateAsync(updatedCountry);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(new { errors });
            }

            try
            {
                _repository.Update(id, updatedCountry);
                _logger.LogInformation("Country updated: {CountryId}", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Country not found for update: {CountryId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating country");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Failed to update country. Please try again." });
            }
        }

        // DELETE api/countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Country ID must be a positive integer" });
            }

            try
            {
                _repository.Delete(id);
                _logger.LogInformation("Country deleted: {CountryId}", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Country not found for deletion: {CountryId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting country");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Failed to delete country. Please try again." });
            }
        }
    }
}

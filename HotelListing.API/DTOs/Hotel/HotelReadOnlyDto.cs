using HotelListing.API.DTOs.Country;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.DTOs.Hotel
{
    public class HotelReadOnlyDto : BaseHotelDto
    {
        [Required]
        public int Id { get; set; }
        public CountryReadOnlyDto? Country { get; set; }
    }
}

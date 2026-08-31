using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.DTOs
{
    public class HotelReadOnlyDto : BaseHotelDto
    {
        [Required]
        public int Id { get; set; }
    }
}

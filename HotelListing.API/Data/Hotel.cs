using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Data
{
    public class Hotel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Address { get; set; }
        public double Rating { get; set; }
    }
}

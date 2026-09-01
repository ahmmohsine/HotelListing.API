using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.DTOs
{
    public class BaseHotelDto
    {
        [Required(ErrorMessage = "Le nom de l'hôtel est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'adresse est obligatoire.")]
        [StringLength(250, ErrorMessage = "L'adresse ne peut pas dépasser 250 caractères.")]
        public string Address { get; set; } = string.Empty;

        [Range(1.0, 5.0, ErrorMessage = "La note doit être comprise entre 1.0 et 5.0.")]
        public double Rating { get; set; }

        [Required(ErrorMessage = "L'identifiant du pays est obligatoire.")]
        [Range(1, int.MaxValue, ErrorMessage = "Veuillez fournir un ID de pays valide.")]
        public int CountryId { get; set; }
    }
}

using HotelListing.API.Data;
using HotelListing.API.DTOs.Country;
using HotelListing.API.DTOs.Hotel;
using HotelListing.API.Repositories;

namespace HotelListing.API.Services;

public class HotelService : IHotelService
{
    private readonly IGenericRepository<Hotel> _hotelRepository;

    public HotelService(IGenericRepository<Hotel> hotelRepository)
    {
        _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
    }

    public async Task<HotelReadOnlyDto> CreateAsync(CreateHotelDto hotelDto, CancellationToken ct = default)
    {
        var hotel = new Hotel
        {
            Name = hotelDto.Name,
            Address = hotelDto.Address,
            Rating = hotelDto.Rating,
            CountryId = hotelDto.CountryId,
        };

        var createdHotel = await _hotelRepository.AddAsync(hotel, ct);
        return MapToReadOnlyDto(createdHotel);
    }

    public async Task<IEnumerable<HotelReadOnlyDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _hotelRepository.GetAllAsync(ct);
        return list.Select(MapToReadOnlyDto);
    }

    public async Task<HotelReadOnlyDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id, ct);
        return hotel is null ? null : MapToReadOnlyDto(hotel);
    }

    public async Task UpdateAsync(int id, UpdateHotelDto dto, CancellationToken ct = default)
    {
        if (!await _hotelRepository.ExistsAsync(id, ct))
        {
            throw new KeyNotFoundException($"Impossible de mettre à jour : l'hôtel avec l'ID {id} n'existe pas.");
        }

        await _hotelRepository.UpdateAsync(MapToHotel(id, dto), ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        bool isDeleted = await _hotelRepository.DeleteAsync(id, ct);
        if (!isDeleted)
        {
            throw new KeyNotFoundException($"Impossible de supprimer : l'hôtel avec l'ID {id} n'existe pas.");
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _hotelRepository.ExistsAsync(id, ct);
    }

    // --- Private Mappers ---
    private static HotelReadOnlyDto MapToReadOnlyDto(Hotel hotel)
    {
        return new HotelReadOnlyDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Address = hotel.Address,
            Rating = hotel.Rating,
            CountryId = hotel.CountryId
        };
    }

    private static Hotel MapToHotel(int id, UpdateHotelDto dto)
    {
        return new Hotel
        {
            Id = id,
            Name = dto.Name,
            Address = dto.Address,
            Rating = dto.Rating,
            CountryId = dto.CountryId,
        };
    }


}
using HotelListing.API.Data;
using HotelListing.API.DTOs;
using HotelListing.API.Repositories;

namespace HotelListing.API.Services
{
    public class HotelService : IHotelService
    {
        private readonly IGenericRepository<Hotel> _hotelRepository;
        public HotelService(IGenericRepository<Hotel> hotelRepository)
        {
            _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
        }
        public async Task<HotelReadOnlyDto> CreateAsync(CreateHotelDto hotalDto, CancellationToken ct = default)
        {
            var hotel = new Hotel()
            {
                Name = hotalDto.Name,
                Address = hotalDto.Address,
                Rating = hotalDto.Rating,
                CountryId = hotalDto.CountryId,
            };
            var createdHotel = await _hotelRepository.AddAsync(hotel, ct);
            return MapToReadOnlyDto(createdHotel);
        }

        private HotelReadOnlyDto MapToReadOnlyDto(Hotel hotel)
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

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var exists = await _hotelRepository.ExistsAsync(id, ct);
            if (!exists) return false;

            await _hotelRepository.DeleteAsync(id, ct);
            return true;
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

        public async Task<bool> UpdateAsync(int id, UpdateHotelDto dto, CancellationToken ct = default)
        {
            if (!await _hotelRepository.ExistsAsync(id))
                return false;
            await _hotelRepository.UpdateAsync(MapToHotel(id, dto), ct);
            return true;
        }

        private Hotel MapToHotel(int id, UpdateHotelDto dto)
        {
            return new Hotel()
            {
                Id = id,
                Name = dto.Name,
                Address = dto.Address,
                Rating = dto.Rating,
                CountryId = dto.CountryId,
            };
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _hotelRepository.ExistsAsync(id, ct);
        }
    }
}

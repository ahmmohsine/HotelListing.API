using HotelListing.API.DTOs;

namespace HotelListing.API.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelReadOnlyDto>> GetAllAsync(CancellationToken ct = default);
        Task<HotelReadOnlyDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateHotelDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<HotelReadOnlyDto> CreateAsync(CreateHotelDto dto, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    }
}

using HotelListing.API.DTOs.Country;
using HotelListing.API.DTOs.Hotel;
using HotelListing.API.Results;

namespace HotelListing.API.Services
{
    public interface IHotelService
    {
        Task<Result<IEnumerable<HotelReadOnlyDto>>> GetAllAsync(CancellationToken ct = default);
        Task<Result<HotelReadOnlyDto>> CreateAsync(CreateHotelDto hotelDto, CancellationToken ct = default);
        Task<Result<HotelReadOnlyDto?>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result> UpdateAsync(int id, UpdateHotelDto dto, CancellationToken ct = default);
        Task<Result> DeleteAsync(int id, CancellationToken ct = default);
        Task<Result<bool>> ExistsAsync(int id, CancellationToken ct = default);

    }
}

using HotelListing.API.Data;
using HotelListing.API.DTOs.Country;
using HotelListing.API.DTOs.Hotel;
using HotelListing.API.Repositories;
using HotelListing.API.Results;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Services;

public class HotelService : IHotelService
{
    private readonly IGenericRepository<Hotel> _hotelRepository;
    private readonly IMapper _mapper;

    public HotelService(IGenericRepository<Hotel> hotelRepository, IMapper mapper)
    {
        _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<HotelReadOnlyDto>> CreateAsync(CreateHotelDto hotelDto, CancellationToken ct = default)
    {
        var hotel = _mapper.Map<Hotel>(hotelDto);
        var createdHotel = await _hotelRepository.AddAsync(hotel, ct);

        return Result<HotelReadOnlyDto>.Success(_mapper.Map<HotelReadOnlyDto>(createdHotel));
    }

    public async Task<Result<IEnumerable<HotelReadOnlyDto>>> GetAllAsync(CancellationToken ct = default)
    {

        var hotels = await _hotelRepository.GetQueryable()
            .ProjectToType<HotelReadOnlyDto>(_mapper.Config)
            .ToListAsync(ct);

        return Result<IEnumerable<HotelReadOnlyDto>>.Success(hotels);
    }

    public async Task<Result<HotelReadOnlyDto?>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id, ct);
        return hotel is null
            ? Result<HotelReadOnlyDto?>.Failure(new Error("Hotel.NotFound", $"L'hôtel avec l'ID {id} n'a pas été trouvé."))
            : Result<HotelReadOnlyDto?>.Success(_mapper.Map<HotelReadOnlyDto>(hotel));
    }

    public async Task<Result> UpdateAsync(int id, UpdateHotelDto dto, CancellationToken ct = default)
    {
        var existingHotel = await _hotelRepository.GetByIdAsync(id, ct);
        if (existingHotel is null)
        {
            return Result.Failure(new Error("Hotel.NotFound", $"L'hôtel avec l'ID {id} n'a pas été trouvé."));
        }

        _mapper.Map(dto, existingHotel);

        await _hotelRepository.UpdateAsync(existingHotel, ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        bool isDeleted = await _hotelRepository.DeleteAsync(id, ct);
        if (!isDeleted)
        {
            return Result.Failure(new Error("Hotel.NotFound", $"L'hôtel avec l'ID {id} n'a pas été trouvé."));
        }

        return Result.Success();
    }

    public async Task<Result<bool>> ExistsAsync(int id, CancellationToken ct = default)
    {
        return Result<bool>.Success(await _hotelRepository.ExistsAsync(id, ct));
    }

}
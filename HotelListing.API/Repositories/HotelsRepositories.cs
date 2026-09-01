using HotelListing.API.Data;
using System.Linq.Expressions;

namespace HotelListing.API.Repositories;

public class HotelsRepository : IGenericRepository<Hotel>
{
    private static readonly List<Hotel> _hotels = Seed.GenerateHotels();

    public Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IEnumerable<Hotel>>(_hotels);
    }

    public Task<Hotel?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var hotel = _hotels.FirstOrDefault(h => h.Id == id);
        return Task.FromResult(hotel);
    }

    public Task<Hotel> AddAsync(Hotel hotel, CancellationToken ct = default)
    {
        int nextId = _hotels.Any() ? _hotels.Max(h => h.Id) + 1 : 1;
        hotel.Id = nextId;

        _hotels.Add(hotel);
        return Task.FromResult(hotel);
    }
    public Task UpdateAsync(Hotel entity, CancellationToken ct = default)
    {
        var existingHotel = _hotels.FirstOrDefault(h => h.Id == entity.Id);
        if (existingHotel == null)
        {
            throw new KeyNotFoundException($"L'hôtel avec l'ID {entity.Id} n'existe pas.");
        }

        existingHotel.Name = entity.Name;
        existingHotel.Address = entity.Address;
        existingHotel.Rating = entity.Rating;
        existingHotel.CountryId = entity.CountryId;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var hotel = _hotels.FirstOrDefault(h => h.Id == id);
        if (hotel != null)
        {
            _hotels.Remove(hotel);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return Task.FromResult(_hotels.Any(h => h.Id == id));
    }


    public Task<Hotel?> FindAsync(Expression<Func<Hotel, bool>> predicate, CancellationToken ct = default)
    {
        var hotel = _hotels.AsQueryable().FirstOrDefault(predicate);
        return Task.FromResult(hotel);
    }
}
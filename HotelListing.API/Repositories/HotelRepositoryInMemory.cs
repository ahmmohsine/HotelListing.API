using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace HotelListing.API.Repositories;

public class HotelRepositoryInMemory : IGenericRepository<Hotel>
{
    private static readonly List<Hotel> _hotels;

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

        // Pas de throw d'exception métier ici : si l'entité existe, on la met à jour.
        if (existingHotel != null)
        {
            existingHotel.Name = entity.Name;
            existingHotel.Address = entity.Address;
            existingHotel.Rating = entity.Rating;
            existingHotel.CountryId = entity.CountryId;
        }

        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var hotel = _hotels.FirstOrDefault(h => h.Id == id);
        if (hotel != null)
        {
            _hotels.Remove(hotel);
            return Task.FromResult(true);
        }

        // Correction du type de retour : Task simple (CompletedTask)
        return Task.FromResult(false);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return Task.FromResult(_hotels.Any(h => h.Id == id));
    }

    public Task<Hotel?> FindAsync(Expression<Func<Hotel, bool>> predicate, CancellationToken ct = default)
    {
        // Compile l'expression LINQ pour l'exécuter sur la liste en mémoire
        var hotel = _hotels.AsQueryable().FirstOrDefault(predicate);
        return Task.FromResult(hotel);
    }

    public IQueryable<Hotel> GetQueryable()
    {
        return _hotels.AsQueryable().AsNoTracking();
    }
}
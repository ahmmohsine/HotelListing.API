using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotelListing.API.Repositories;

public class HotelRepository : IGenericRepository<Hotel>
{
    private readonly HotelListingDbContext _context;

    public HotelRepository(HotelListingDbContext context)
    {
        _context = context;
    }

    public async Task<Hotel> AddAsync(Hotel hotel, CancellationToken ct = default)
    {
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync(ct); // Persistance effective en BDD
        return hotel;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var hotel = await _context.Hotels.FindAsync(new object[] { id }, ct);
        if (hotel != null)
        {
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync(ct);
            return true;
        }
        return false;
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _context.Hotels.AnyAsync(h => h.Id == id, ct);
    }

    public async Task<Hotel?> FindAsync(Expression<Func<Hotel, bool>> predicate, CancellationToken ct = default)
    {
        return await _context.Hotels
            .AsNoTracking() // Performance : pas de Change Tracking pour la lecture
            .FirstOrDefaultAsync(predicate, ct);
    }

    public async Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Hotels
            .AsNoTracking() // Performance
            .ToListAsync(ct);
    }

    public async Task<Hotel?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Hotels.FindAsync(new object[] { id }, ct);
    }

    public async Task UpdateAsync(Hotel entity, CancellationToken ct = default)
    {
        _context.Hotels.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

}
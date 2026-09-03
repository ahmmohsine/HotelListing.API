using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotelListing.API.Repositories
{
    public class CountryRepository : IGenericRepository<Country>
    {
        HotelListingDbContext _context;
        public CountryRepository(HotelListingDbContext context)
        {
            _context = context;
        }
        public async Task<Country> AddAsync(Country country, CancellationToken ct = default)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync(ct);
            return country;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var country = await _context.Countries.FindAsync(new object[] { id }, ct);
            if (country == null)
                return false;

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            var country = await _context.Countries.FindAsync(new object[] { id }, ct);
            return country != null;
        }

        public async Task<Country?> FindAsync(Expression<Func<Country, bool>> predicate, CancellationToken ct = default)
        {
            return await _context.Countries
               .AsNoTracking()
            .FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<IEnumerable<Country>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Countries
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<Country?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Countries.FindAsync(new object[] { id }, ct);
        }

        public async Task UpdateAsync(Country entity, CancellationToken ct = default)
        {
            _context.Countries.Update(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}

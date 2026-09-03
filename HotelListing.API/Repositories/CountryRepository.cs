using HotelListing.API.Data;
using System.Linq.Expressions;

namespace HotelListing.API.Repositories
{
    public class CountryRepository : IGenericRepository<Country>
    {
        private static readonly List<Country> _countries = Seed.GenerateCountries();
        private static readonly object _lock = new();

        public Task<IEnumerable<Country>> GetAllAsync(CancellationToken ct = default)
        {
            lock (_lock)
            {
                return Task.FromResult<IEnumerable<Country>>(_countries.ToList());
            }
        }

        public Task<Country?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            lock (_lock)
            { return Task.FromResult(_countries.FirstOrDefault(c => c.Id == id)); }
        }

        public Task UpdateAsync(Country country, CancellationToken ct = default)
        {
            lock (_lock)
            {
                var _country = _countries.FirstOrDefault(c => c.Id == country.Id);
                if (_country == null)
                {
                    throw new KeyNotFoundException($"Le pays avec l'ID {country.Id} n'existe pas.");
                }
                _country.Name = country.Name;
                _country.Code = country.Code;
                return Task.CompletedTask;
            }
        }

        public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            lock (_lock)
            {
                var _country = _countries.FirstOrDefault(c => c.Id == id);
                if (_country == null)
                {
                    return Task.FromResult(false);
                }

                _countries.Remove(_country);
                return Task.FromResult(true);
            }
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            lock (_lock)
            { return Task.FromResult(_countries.Any(h => h.Id == id)); }

        }



        public Task<Country> AddAsync(Country country, CancellationToken ct = default)
        {
            lock (_lock)
            {
                int newId = _countries.Any() ? _countries.Max(c => c.Id) + 1 : 1;
                country.Id = newId;
                _countries.Add(country);
                return Task.FromResult(country);
            }
        }
        public Task<Country?> FindAsync(Expression<Func<Country, bool>> predicate, CancellationToken ct = default)
        {
            lock (_lock)
            {
                var result = _countries.AsQueryable().FirstOrDefault(predicate);
                return Task.FromResult(result);
            }
        }

    }
}

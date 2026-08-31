using HotelListing.API.Data;

namespace HotelListing.API.Repositories
{
    public class CountryRepository : IGenericRepository<Country>
    {
        private static readonly List<Country> _countries = new List<Country>
        {
            new Country { Id = 1, Name = "United States", Code = "US" },
            new Country { Id = 2, Name = "Canada", Code = "CA" },
            new Country { Id = 3, Name = "Mexico", Code = "MX" },
            new Country { Id = 4, Name = "United Kingdom", Code = "UK" },
            new Country { Id = 5, Name = "France", Code = "FR" }
        };

        public Task<IEnumerable<Country>> GetAllAsync(CancellationToken ct = default)
        {
            return Task.FromResult<IEnumerable<Country>>(_countries);
        }

        public Task<Country?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return Task.FromResult(_countries.FirstOrDefault(c => c.Id == id));

        }

        public Task UpdateAsync(Country country, CancellationToken ct = default)
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

        public Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var _country = _countries.FirstOrDefault(c => c.Id == id);
            if (_country == null)
            {
                throw new KeyNotFoundException($"Country with Id {id} not found");
            }

            _countries.Remove(_country);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return Task.FromResult(_countries.Any(h => h.Id == id));
        }


        public Task<Country> AddAsync(Country country, CancellationToken ct = default)
        {

            if (_countries.Any(c => c.Id == country.Id))
            {
                throw new InvalidOperationException($"Country with Id {country.Id} already exists");
            }
            _countries.Add(country);
            return Task.FromResult(country);
        }
    }
}

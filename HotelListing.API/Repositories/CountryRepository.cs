using HotelListing.API.Data;

namespace HotelListing.API.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private static readonly List<Country> _countries = new List<Country>
        {
            new Country { Id = 1, Name = "United States", Code = "US" },
            new Country { Id = 2, Name = "Canada", Code = "CA" },
            new Country { Id = 3, Name = "Mexico", Code = "MX" },
            new Country { Id = 4, Name = "United Kingdom", Code = "UK" },
            new Country { Id = 5, Name = "France", Code = "FR" }
        };

        public IEnumerable<Country> GetAll()
        {
            return _countries;
        }

        public Country? GetById(int id)
        {
            return _countries.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Country country)
        {
            if (Exists(country.Id))
            {
                throw new InvalidOperationException($"Country with Id {country.Id} already exists");
            }
            _countries.Add(country);
        }

        public void Update(int id, Country country)
        {
            var existingCountry = GetById(id);
            if (existingCountry == null)
            {
                throw new KeyNotFoundException($"Country with Id {id} not found");
            }

            existingCountry.Name = country.Name;
            existingCountry.Code = country.Code;
        }

        public void Delete(int id)
        {
            var country = GetById(id);
            if (country == null)
            {
                throw new KeyNotFoundException($"Country with Id {id} not found");
            }

            _countries.Remove(country);
        }

        public bool Exists(int id)
        {
            return _countries.Any(c => c.Id == id);
        }
    }
}

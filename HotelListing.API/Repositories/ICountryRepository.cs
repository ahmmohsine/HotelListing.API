using HotelListing.API.Data;

namespace HotelListing.API.Repositories
{
    public interface ICountryRepository
    {
        IEnumerable<Country> GetAll();
        Country? GetById(int id);
        void Add(Country country);
        void Update(int id, Country country);
        void Delete(int id);
        bool Exists(int id);
    }
}

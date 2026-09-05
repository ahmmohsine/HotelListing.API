using HotelListing.API.Data;
using HotelListing.API.DTOs.Hotel;
using Mapster;

namespace HotelListing.API.MappingProfiles
{
    public class MappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Hotel, HotelReadOnlyDto>()
              .Map(dest => dest.Country, src => src.Country.Name).TwoWays();
        }
    }
}

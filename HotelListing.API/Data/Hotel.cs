using System;
using System.Collections.Generic;

namespace HotelListing.API.Data;

public partial class Hotel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public double Rating { get; set; }

    public int CountryId { get; set; }

    public virtual Country Country { get; set; } = null!;
}

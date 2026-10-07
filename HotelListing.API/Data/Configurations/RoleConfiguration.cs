using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "01a116d7-71e7-70ca-af23-4dbba9048a6a",
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = "01a116d7-71e7-70ca-af23-4dbba9048a6a"
            },
            new IdentityRole
            {
                Id = "01a116d7-71e7-7c32-85e6-13511cf1539b",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                ConcurrencyStamp = "01a116d7-71e7-7c32-85e6-13511cf1539b"
            }
    );
    }
}


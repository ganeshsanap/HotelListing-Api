using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Data.Configurations.Entities
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                new Hotel()
                {
                    Id = 1,
                    Name = "Hyatt",
                    Address = "Mumbai",
                    Rating = 4.5,
                    CountryId = 1
                },
                new Hotel()
                {
                    Id = 2,
                    Name = "Embassy Suites",
                    Address = "Cape Town",
                    Rating = 4.2,
                    CountryId = 2
                },
                new Hotel()
                {
                    Id = 3,
                    Name = "Ritz-Carlton",
                    Address = "Moscow",
                    Rating = 4.0,
                    CountryId = 3
                }
                );
        }
    }
}

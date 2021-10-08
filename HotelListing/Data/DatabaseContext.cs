using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Country>().HasData(
                new Country()
                {
                    Id = 1,
                    Name = "India",
                    ShortName = "IND"
                },
                new Country()
                {
                    Id = 2,
                    Name = "South Africa",
                    ShortName = "SA"
                }, 
                new Country()
                {
                    Id = 3,
                    Name = "Russia",
                    ShortName = "RUS"
                }
            );

            builder.Entity<Hotel>().HasData(
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
            ); ;
        }

    }
}

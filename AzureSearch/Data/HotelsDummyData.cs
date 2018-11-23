using AzureSearch.Infrastructure.Models;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;

namespace AzureSearch.Data
{
    public static class HotelsDummyData
    {
        public static IEnumerable<IndexAction<Hotel>> GenerateActions()
        {
            return new IndexAction<Hotel>[]
           {
                IndexAction.Upload(
                    new Hotel()
                    {
                        HotelId = "1",
                        BaseRate = 199.0,
                        Description = "Best hotel in town",
                        DescriptionFr = "Meilleur hôtel en ville",
                        HotelName = "Fancy Stay",
                        Category = "Luxury",
                        Tags = new[] { "pool", "view", "wifi", "concierge" },
                        ParkingIncluded = false,
                        SmokingAllowed = false,
                        LastRenovationDate = new DateTimeOffset(2010, 6, 27, 0, 0, 0, TimeSpan.Zero),
                        Rating = 5,
                        Location = GeographyPoint.Create(47.678581, -122.131577)
                    }),
                IndexAction.Upload(
                    new Hotel()
                    {
                        HotelId = "2",
                        BaseRate = 79.99,
                        Description = "Cheapest hotel in town",
                        DescriptionFr = "Hôtel le moins cher en ville",
                        HotelName = "Roach Motel",
                        Category = "Budget",
                        Tags = new[] { "motel", "budget" },
                        ParkingIncluded = true,
                        SmokingAllowed = true,
                        LastRenovationDate = new DateTimeOffset(1982, 4, 28, 0, 0, 0, TimeSpan.Zero),
                        Rating = 1,
                        Location = GeographyPoint.Create(49.678581, -122.131577)
                    }),
                IndexAction.MergeOrUpload(
                    new Hotel()
                    {
                        HotelId = "3",
                        BaseRate = 129.99,
                        Description = "Close to town hall and the river",
                        HotelName = "Super Fancy Stay",
                        Category = "Luxury",
                    }),
                IndexAction.Delete(new Hotel() { HotelId = "6" })
           };
        }
    }
}

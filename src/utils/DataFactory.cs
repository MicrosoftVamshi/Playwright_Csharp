using System;

namespace Demoblaze.Tests.src.utils
{
    public static class DataFactory
    {
        public static (string Username, string Password) UniqueUser(string prefix)
        {
            var ts = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var rnd = new Random().Next(100000);
            return ($"{prefix}_{ts}_{rnd}", $"Pass@{ts}_{rnd}");
        }

        public static PurchaseDetails Purchase()
        {
            return new PurchaseDetails
            {
                Name = "Vamshi",
                Country = "India",
                City = "Hyderabad",
                Card = "4111111111111111",
                Month = "04",
                Year = "2030"
            };
        }
    }

    public class PurchaseDetails
    {
        public string Name { get; set; } = "";
        public string Country { get; set; } = "";
        public string City { get; set; } = "";
        public string Card { get; set; } = "";
        public string Month { get; set; } = "";
        public string Year { get; set; } = "";
    }
}

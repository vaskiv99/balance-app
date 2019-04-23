using System;

namespace TradeLib.Models
{
    public class TradeOffer
    {
        public string ClientName { get; set; }

        public decimal Price { get; set; }

        public DateTimeOffset ExpiredDate { get; set; }
    }
}
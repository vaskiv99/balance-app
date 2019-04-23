using System;

namespace TradeLib.Models
{
    public class Order
    {
        public string SellerName { get; set; }

        public string BuyerName { get; set; }

        public decimal Price { get; set; }

        public DateTimeOffset Date { get; set; }

        public override string ToString()
        {
            return $"Seller: {SellerName} - Buyer: {BuyerName} - Price: {Price}";
        }
    }
}
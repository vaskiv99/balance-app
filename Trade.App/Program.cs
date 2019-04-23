using System;
using TradeLib.Implementations;
using TradeLib.Models;

namespace Trade.App
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var tradeOffer = new TradeOffer { ClientName = "A", Price = 10, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
            var tradeOffer2 = new TradeOffer { ClientName = "B", Price = 11, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
            var tradeOffer3 = new TradeOffer { ClientName = "C", Price = 15, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
            var tradeOffer4 = new TradeOffer { ClientName = "D", Price = 9, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
            var tradeOffer5 = new TradeOffer { ClientName = "E", Price = 10, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
            var tradeOffer6 = new TradeOffer { ClientName = "F", Price = 10, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
            var tradeOffer7 = new TradeOffer { ClientName = "G", Price = 100, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };

            var service = new TradeService();

            service.BuyOffer(tradeOffer, 12);
            service.BuyOffer(tradeOffer2, 12);
            service.SellOffer(tradeOffer3, 12);
            service.SellOffer(tradeOffer4, 12);
            service.BuyOffer(tradeOffer5, 12);
            service.SellOffer(tradeOffer6, 12);
            service.BuyOffer(tradeOffer7, 12);

            foreach (var order in service.GetTradeOrders())
            {
                Console.WriteLine(order);
            }

            Console.ReadKey();
        }
    }
}

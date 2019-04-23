using System.Collections.Generic;
using TradeLib.Models;

namespace TradeLib.Abstractions
{
    public interface ITradeService
    {
        void BuyOffer(TradeOffer tradeOffer, int countOfProducts);

        void SellOffer(TradeOffer tradeOffer, int countOfProducts);

        IEnumerable<Order> GetTradeOrders();
    }
}
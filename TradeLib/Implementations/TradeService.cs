using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TradeLib.Abstractions;
using TradeLib.Models;

namespace TradeLib.Implementations
{
    public class TradeService : ITradeService
    {
        private readonly ConcurrentDictionary<Guid, ExtendedTradeOffer> _offerBuyItems;
        private readonly ConcurrentDictionary<Guid, ExtendedTradeOffer> _offerSellItems;
        private readonly ConcurrentBag<Order> _orders;

        public TradeService()
        {
            _offerBuyItems = new ConcurrentDictionary<Guid, ExtendedTradeOffer>();
            _offerSellItems = new ConcurrentDictionary<Guid, ExtendedTradeOffer>();
            _orders = new ConcurrentBag<Order>();
        }

        public void BuyOffer(TradeOffer tradeOffer, int countOfProducts = 1)
        {
            if (tradeOffer == null)
            {
                throw new ArgumentNullException(nameof(tradeOffer));
            }

            if (tradeOffer.ExpiredDate <= DateTimeOffset.UtcNow)
            {
                throw new ArgumentOutOfRangeException(nameof(tradeOffer.ExpiredDate), "Expired date must be in the future.");
            }

            if (tradeOffer.Price <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tradeOffer.Price), "Price must be greater than 0.");
            }

            var sellOffer = _offerSellItems.OrderBy(x => x.Value.Price).ThenBy(x => x.Value.CreatedDate)
                .FirstOrDefault(x => x.Value.Price <= tradeOffer.Price && DateTimeOffset.UtcNow <= x.Value.ExpiredDate);

            if (sellOffer.Value == null)
            {
                _offerBuyItems.TryAdd(Guid.NewGuid(), ExtendedTradeOffer.ToExtendedTradeItem(tradeOffer));

                return;
            }

            _orders.Add(new Order
            {
                BuyerName = tradeOffer.ClientName,
                Price = IsLatest(x => x.Price <= tradeOffer.Price && DateTimeOffset.UtcNow <= x.ExpiredDate) ? tradeOffer.Price : sellOffer.Value.Price,
                SellerName = sellOffer.Value.ClientName,
                Date = DateTimeOffset.UtcNow
            });

            _offerSellItems.TryRemove(sellOffer.Key, out _);
        }

        public void SellOffer(TradeOffer tradeOffer, int countOfProducts = 1)
        {
            if (tradeOffer == null)
            {
                throw new ArgumentNullException(nameof(tradeOffer));
            }

            if (tradeOffer.ExpiredDate <= DateTimeOffset.UtcNow)
            {
                throw new ArgumentOutOfRangeException(nameof(tradeOffer.ExpiredDate), "Expired date must be in the future.");
            }

            if (tradeOffer.Price <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tradeOffer.Price), "Price must be greater than 0.");
            }

            var buyOffer = _offerBuyItems.OrderByDescending(x => x.Value.Price).ThenBy(x => x.Value.CreatedDate)
                .FirstOrDefault(x => x.Value.Price >= tradeOffer.Price && DateTimeOffset.UtcNow <= x.Value.ExpiredDate);

            if (buyOffer.Value == null)
            {
                _offerSellItems.TryAdd(Guid.NewGuid(), ExtendedTradeOffer.ToExtendedTradeItem(tradeOffer));

                return;
            }

            _orders.Add(new Order { BuyerName = buyOffer.Value.ClientName, Price = tradeOffer.Price, SellerName = tradeOffer.ClientName, Date = DateTimeOffset.UtcNow });
            _offerBuyItems.TryRemove(buyOffer.Key, out _);
        }

        public IEnumerable<Order> GetTradeOrders() => _orders;

        private bool IsLatest(Func<ExtendedTradeOffer, bool> expression)
        {
            return _offerSellItems.Values.Count(expression) <= 1;
        }
    }
}
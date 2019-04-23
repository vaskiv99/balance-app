using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeLib.Implementations;
using TradeLib.Models;

namespace Trade.Tests
{
    [TestClass]
    public class TradeTest
    {
        #region Trade Offer objects

        private static TradeOffer NegativePriceTradeOffer => new TradeOffer { ClientName = "A", Price = -10, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
        private static TradeOffer NegativeDateTradeOffer => new TradeOffer { ClientName = "B", Price = 11, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(-1) };
        private static TradeOffer ValidTradeOfferWithExpiredDate => new TradeOffer { ClientName = "D", Price = 15, ExpiredDate = DateTimeOffset.UtcNow.AddSeconds(3) };
        private static TradeOffer ValidTradeOfferClientC => new TradeOffer { ClientName = "C", Price = 15, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
        private static TradeOffer ValidTradeOfferClientD => new TradeOffer { ClientName = "D", Price = 9, ExpiredDate = DateTimeOffset.UtcNow.AddSeconds(3) };
        private static TradeOffer ValidTradeOfferE => new TradeOffer { ClientName = "E", Price = 10, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
        private static TradeOffer ValidTradeOfferClientF => new TradeOffer { ClientName = "F", Price = 10, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
        private static TradeOffer ValidTradeOfferClientG => new TradeOffer { ClientName = "G", Price = 100, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
        private static TradeOffer ValidTradeOfferClientA => new TradeOffer { ClientName = "A", Price = 10, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };
        private static TradeOffer ValidTradeOfferClientB => new TradeOffer { ClientName = "B", Price = 11, ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(1) };

        #endregion

        #region Validators

        [TestMethod]
        public void BuyOffer_ThrowArgumentNullException()
        {
            var service = new TradeService();

            Assert.ThrowsException<ArgumentNullException>(() => service.BuyOffer(null));
        }

        [TestMethod]
        public void BuyOffer_NegativePrice_ThrowArgumentOutOfRangeException()
        {
            var service = new TradeService();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => service.BuyOffer(NegativePriceTradeOffer));
        }

        [TestMethod]
        public void BuyOffer_NegativeDate_ThrowArgumentOutOfRangeException()
        {
            var service = new TradeService();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => service.BuyOffer(NegativeDateTradeOffer));
        }

        #endregion

        #region Orders

        [TestMethod]
        public void BuyOffer_OnlyBuyOffers_OrdersEmpty()
        {
            var service = new TradeService();
            service.BuyOffer(ValidTradeOfferClientC);

            var orders = service.GetTradeOrders();

            Assert.IsNotNull(orders);
            Assert.AreEqual(0, orders.Count());
        }

        [TestMethod]
        public void SellOffer_OnlySellOffers_OrdersEmpty()
        {
            var service = new TradeService();
            service.SellOffer(ValidTradeOfferClientC);

            var orders = service.GetTradeOrders();

            Assert.IsNotNull(orders);
            Assert.AreEqual(0, orders.Count());
        }

        [TestMethod]
        public void GetOrders_OneTrade_ValidOrders()
        {
            var service = new TradeService();
            service.SellOffer(ValidTradeOfferClientC);
            service.BuyOffer(ValidTradeOfferWithExpiredDate);

            var orders = service.GetTradeOrders();

            Assert.IsNotNull(orders);
            var enumerable = orders.ToList();
            Assert.AreEqual(1, enumerable.Count);
            Assert.IsTrue(enumerable.Any(x => x.Price == ValidTradeOfferWithExpiredDate.Price && x.SellerName == ValidTradeOfferClientC.ClientName));
        }

        [TestMethod]
        public async Task GetOrders_OfferExpired_ValidOrders()
        {
            var service = new TradeService();
            service.SellOffer(ValidTradeOfferWithExpiredDate);

            await Task.Delay(4000).ConfigureAwait(false);

            service.BuyOffer(ValidTradeOfferClientC);

            var orders = service.GetTradeOrders();

            Assert.IsNotNull(orders);
            Assert.AreEqual(0, orders.Count());
        }

        [TestMethod]
        public void GetOrders_ManyTrades_ValidOrders()
        {
            var service = new TradeService();
            service.BuyOffer(ValidTradeOfferClientA);
            service.BuyOffer(ValidTradeOfferClientB);
            service.SellOffer(ValidTradeOfferClientC);
            service.SellOffer(ValidTradeOfferClientD);
            service.BuyOffer(ValidTradeOfferE);
            service.SellOffer(ValidTradeOfferClientF);
            service.BuyOffer(ValidTradeOfferClientG);

            var orders = service.GetTradeOrders();

            Assert.IsNotNull(orders);
            var enumerable = orders.ToList();
            Assert.AreEqual(3, enumerable.Count);
            Assert.IsTrue(enumerable.Any(x => x.Price == 100 && x.BuyerName == ValidTradeOfferClientG.ClientName));
        }

        #endregion
    }
}

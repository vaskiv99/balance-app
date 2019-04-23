using System;

namespace TradeLib.Models
{
    public class ExtendedTradeOffer : TradeOffer
    {
        public DateTimeOffset CreatedDate { get; set; }

        public static ExtendedTradeOffer ToExtendedTradeItem(TradeOffer tradeOffer) => new ExtendedTradeOffer
        {
            CreatedDate = DateTimeOffset.UtcNow,
            ClientName = tradeOffer.ClientName,
            Price = tradeOffer.Price,
            ExpiredDate = tradeOffer.ExpiredDate
        };
    }
}
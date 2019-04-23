using System;
using Balance.Models.Enums;

namespace Balance.Models.Transactions
{
    public class TransactionResponse
    {
        public long Id { get; set; }

        public long RecipientId { get; set; }

        public long SenderId { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public decimal Amount { get; set; }

        public TransactionType TransactionType { get; set; }
    }
}
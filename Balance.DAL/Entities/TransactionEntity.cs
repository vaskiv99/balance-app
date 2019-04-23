using System;
using Balance.Models.Enums;

namespace Balance.DAL.Entities
{
    public class TransactionEntity
    {
        public long Id { get; set; }

        public long RecipientId { get; set; }

        public UserEntity Recipient { get; set; }

        public UserEntity Sender { get; set; }

        public long SenderId { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public decimal Amount { get; set; }

        public TransactionType TransactionType { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Balance.Models.Transactions
{
    public class TransactionRequest 
    {
        public long RecipientId { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Amount { get; set; }
    }
}
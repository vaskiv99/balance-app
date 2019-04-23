using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Balance.DAL.Entities
{
    public class UserEntity 
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        [ConcurrencyCheck]
        public decimal Balance { get; set; }

        public List<TransactionEntity> Transactions { get; set; }
    }
}
using System;
using Balance.DAL.Entities;
using Balance.Models.Enums;
using Balance.Models.Transactions;

namespace Balance.Services.Extensions
{
    public static class TransactionExtension
    {
        public static TransactionEntity ToEntity(this TransactionRequest request, TransactionType transactionType,
            long senderId) => new TransactionEntity
        {
            Amount = request.Amount,
            CreatedDate = DateTimeOffset.UtcNow,
            RecipientId = request.RecipientId,
            SenderId = senderId,
            TransactionType = transactionType
        };

        public static TransactionResponse ToResponse(this TransactionEntity entity) => new TransactionResponse
        {
            Id = entity.Id,
            Amount = entity.Amount,
            CreatedDate = entity.CreatedDate,
            RecipientId = entity.RecipientId,
            SenderId = entity.SenderId,
            TransactionType = entity.TransactionType
        };
    }
}
using System;
using VoiceAPI.Entities.Enums;

namespace VoiceAPI.Models.Responses.TransactionHistories
{
    public class TransactionHistoryGetDTO
    {
        public Guid Id { get; set; }

        public TransactionTypeEnum TransactionType { get; set; }

        public decimal Amount { get; set; }

        public string AdminEmail { get; set; }

        public string JobName { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
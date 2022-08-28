using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VoiceAPI.Entities
{
    [Table(nameof(Wallet))]
    [Index(nameof(DepositCode), IsUnique = true)]
    public class Wallet
    {
        [Key]
        public Guid Id { get; set; }

        public decimal AvailableBalance { get; set; }

        public decimal LockedBalance { get; set; }
        
        [MaxLength(10), MinLength(10)]
        public string DepositCode { get; set; }

        [ForeignKey(nameof(Id))]
        public Account Account { get; set; }

        public List<TransactionHistory> TransactionHistories { get; set; }
    }
}

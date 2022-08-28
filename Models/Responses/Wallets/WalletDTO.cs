using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Responses.Wallets
{
    public class WalletDTO
    {
        public Guid Id { get; set; }

        public decimal AvailableBalance { get; set; }

        public decimal LockedBalance { get; set; }
        
        public string DepositCode { get; set; }
    }
}

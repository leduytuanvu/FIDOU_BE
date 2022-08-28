﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities.Enums;
using VoiceAPI.Models.Responses.Wallets;

namespace VoiceAPI.Models.Responses.Accounts
{
    public class AccountWithWalletDTO
    {
        public Guid Id { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public AccountStatusEnum Status { get; set; }

        public RoleEnum Role { get; set; }

        public WalletDTO Wallet { get; set; }
    }
}

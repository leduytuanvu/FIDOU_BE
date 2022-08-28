using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;
using VoiceAPI.IRepositories;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Payload.Accounts;
using VoiceAPI.Models.Responses.Accounts;
using VoiceAPI.Models.Responses.Wallets;

namespace VoiceAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;

        private readonly IAccountRepository _accountRepository;

        private readonly IWalletService _walletService;

        public AccountService(IMapper mapper,
            IAccountRepository accountRepository,
            IWalletService walletService)
        {
            _mapper = mapper;

            _accountRepository = accountRepository;

            _walletService = walletService;
        }

        public async Task<GenericResult<AccountWithWalletDTO>> CreateAccount(AccountCreatePayload payload)
        {
            var targetAccount = _mapper.Map<Account>(payload);

            targetAccount.Status = AccountStatusEnum.INACTIVE;

            targetAccount.CreatedTime = DateTime.UtcNow;

            var hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(payload.Password);
            targetAccount.Password = hashPassword;

            _accountRepository.Create(targetAccount);
            await _accountRepository.SaveAsync();

            // Create Wallet
            var targetWallet = await _walletService.CreateNew(targetAccount.Id);

            var response = _mapper.Map<AccountWithWalletDTO>(targetAccount);
            response.Wallet = _mapper.Map<WalletDTO>(targetWallet.Data);

            return GenericResult<AccountWithWalletDTO>.Success(response);
        }

        public async Task<GenericResult<AccountDTO>> GetById(Guid id)
        {
            var account = await _accountRepository.GetById(id);

            if (account == null)
                return GenericResult<AccountDTO>.Error((int)HttpStatusCode.NotFound,
                    "Account is not found.");

            var response = _mapper.Map<AccountDTO>(account);
            
            if (account.Role == RoleEnum.CANDIDATE)
            {
                var targetCandidate = await _accountRepository.GetCandidateById(account.Id);
                response.AvatarUrl = targetCandidate?.AvatarUrl;
            }
            else
            {
                var targetEnterprise = await _accountRepository.GetEnterpriseById(account.Id);
                response.LogoUrl = targetEnterprise?.LogoUrl;
            }

            return GenericResult<AccountDTO>.Success(response);
        }

        public async Task<GenericResult<AccountDTO>> UpdateAccount(AccountUpdatePayload payload)
        {
            var account = await _accountRepository.GetById(payload.Id);

            if (account == null)
                return GenericResult<AccountDTO>.Error((int)HttpStatusCode.NotFound,
                    "Account is not found.");

            account = _mapper.Map<Account>(payload);

            _accountRepository.Update(account);
            await _accountRepository.SaveAsync();

            var response = _mapper.Map<AccountDTO>(account);

            return GenericResult<AccountDTO>.Success(response);
        }

        public async Task<GenericResult<AccountDTO>> UpdateAccountStatus(Guid id, AccountStatusEnum status)
        {
            var targetAccount = await _accountRepository.GetById(id);

            targetAccount.Status = status;

            _accountRepository.Update(targetAccount);
            await _accountRepository.SaveAsync();

            var response = _mapper.Map<AccountDTO>(targetAccount);

            return GenericResult<AccountDTO>.Success(response);
        }

        public async Task<GenericResult<List<AccountDTO>>> GetAll()
        {
            var targetAccounts = await _accountRepository
                .Get()
                .AsNoTracking()
                .ToListAsync();

            var response = _mapper.Map<List<AccountDTO>>(targetAccounts);
            
            foreach (var tempAccount in response)
            {
                if (tempAccount.Role == RoleEnum.CANDIDATE)
                {
                    var targetCandidate = await _accountRepository.GetCandidateById(tempAccount.Id);
                    tempAccount.AvatarUrl = targetCandidate?.AvatarUrl;
                }
                else
                {
                    var targetEnterprise = await _accountRepository.GetEnterpriseById(tempAccount.Id);
                    tempAccount.LogoUrl = targetEnterprise?.LogoUrl;
                }
            }

            return GenericResult<List<AccountDTO>>.Success(response);
        }
    }
}
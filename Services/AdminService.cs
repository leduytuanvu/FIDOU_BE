using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.IRepositories;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Payload.Accounts;
using VoiceAPI.Models.Responses.Accounts;

namespace VoiceAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly IMapper _mapper;

        private readonly IAccountRepository _accountRepository;
        private readonly IAdminRepository _adminRepository;

        public AdminService(IMapper mapper, 
            IAccountRepository accountRepository, 
            IAdminRepository adminRepository)
        {
            _mapper = mapper;

            _accountRepository = accountRepository;
            _adminRepository = adminRepository;
        }

        public async Task<GenericResult<AccountDTO>> ApproveAccount(Guid id)
        {
            var account = await _accountRepository.GetById(id);

            if (account == null)
                return GenericResult<AccountDTO>.Error((int)HttpStatusCode.NotFound,
                    "Account is not found.");

            if (account.Status == Entities.Enums.AccountStatusEnum.ACTIVE)
                return GenericResult<AccountDTO>.Error("V400_02",
                    "Tài khoản này đã được phê duyệt.");

            if (account.Status == Entities.Enums.AccountStatusEnum.DELETED)
                return GenericResult<AccountDTO>.Error("V400_06",
                    "Không thể phê duyệt tài khoản đã bị xóa.");

            account.Status = Entities.Enums.AccountStatusEnum.ACTIVE;

            _accountRepository.Update(account);
            await _accountRepository.SaveAsync();

            var response = _mapper.Map<AccountDTO>(account);

            return GenericResult<AccountDTO>.Success(response);
        }

        public async Task<GenericResult<AccountDTO>> BlockAccount(Guid id)
        {
            var account = await _accountRepository.GetById(id);

            if (account == null)
                return GenericResult<AccountDTO>.Error((int)HttpStatusCode.NotFound,
                    "Account is not found.");

            if (account.Status == Entities.Enums.AccountStatusEnum.BLOCKED)
                return GenericResult<AccountDTO>.Error("V400_03",
                    "Tài khoản này đã bị khóa.");

            if (account.Status == Entities.Enums.AccountStatusEnum.DELETED)
                return GenericResult<AccountDTO>.Error("V400_04",
                    "Không thể khóa tài khoản đã bị xóa.");

            account.Status = Entities.Enums.AccountStatusEnum.BLOCKED;

            _accountRepository.Update(account);
            await _accountRepository.SaveAsync();

            var response = _mapper.Map<AccountDTO>(account);

            return GenericResult<AccountDTO>.Success(response);
        }

        public async Task<GenericResult<AccountDTO>> DeleteAccount(Guid id)
        {
            var account = await _accountRepository.GetById(id);

            if (account == null)
                return GenericResult<AccountDTO>.Error((int)HttpStatusCode.NotFound,
                    "Account is not found.");

            if (account.Status == Entities.Enums.AccountStatusEnum.DELETED)
                return GenericResult<AccountDTO>.Error("V400_05",
                    "Tài khoản này đã bị xóa.");

            account.Status = Entities.Enums.AccountStatusEnum.DELETED;

            _accountRepository.Update(account);
            await _accountRepository.SaveAsync();

            var response = _mapper.Map<AccountDTO>(account);

            return GenericResult<AccountDTO>.Success(response);
        }

        public async Task<GenericResult<Admin>> GetById(Guid id)
        {
            var targetAdmin = await _adminRepository.GetById(id);

            return GenericResult<Admin>.Success(targetAdmin);
        }

        public async Task<GenericResult<AccountDTO>> UnblockAccount(AccountUpdateStatusPayload payload)
        {
            var targetAccount = await _accountRepository.GetById(payload.AccountId);

            targetAccount.Status = payload.Status;

            _accountRepository.Update(targetAccount);
            await _accountRepository.SaveAsync();

            var response = _mapper.Map<AccountDTO>(targetAccount);

            return GenericResult<AccountDTO>.Success(response);
        }
    }
}

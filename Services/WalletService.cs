using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using VoiceAPI.Entities;
using VoiceAPI.IRepositories;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Responses.Wallets;

namespace VoiceAPI.Services
{
    public class WalletService : IWalletService
    {
        private readonly IMapper _mapper;

        private readonly IWalletRepository _walletRepository;

        public WalletService(IMapper mapper, 
            IWalletRepository walletRepository)
        {
            _mapper = mapper;

            _walletRepository = walletRepository;
        }

        private async Task<string> GenerateDepositCode()
        {
            var wallets = await GetAll();

            var isPassed = false;
            var depositCode = new String("");
            
            do
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[10];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                depositCode = new String(stringChars);

                if (!wallets.Data
                    .AsEnumerable()
                    .Select(tempWallet => tempWallet.DepositCode)
                    .ToList()
                    .Contains(depositCode))
                {
                    isPassed = true;
                }
            } while (!isPassed);

            return depositCode;
        }

        public async Task<GenericResult<WalletDTO>> CreateNew(Guid id)
        {
            var targetWallet = await _walletRepository.GetById(id);

            if (targetWallet != null)
                return GenericResult<WalletDTO>.Error((int)HttpStatusCode.BadRequest,
                                            "V400_48",
                                            "Ví này đã có trong hệ thống.");

            targetWallet = new Wallet
            {
                Id = id, 
                DepositCode = await GenerateDepositCode()
            };

            _walletRepository.Create(targetWallet);
            await _walletRepository.SaveAsync();

            var response = _mapper.Map<WalletDTO>(targetWallet);

            return GenericResult<WalletDTO>.Success(response);
        }

        public async Task<GenericResult<List<WalletWithTransactionsDTO>>> GetAll()
        {
            var targetWallets = await _walletRepository.GetAllWalletWithTransactions();

            var response = _mapper.Map<List<WalletWithTransactionsDTO>>(targetWallets);

            return GenericResult<List<WalletWithTransactionsDTO>>.Success(response);
        }

        public async Task<GenericResult<WalletWithTransactionsDTO>> GetById(Guid id)
        {
            var targetWallet = await _walletRepository.GetWalletWithTransactions(id);

            if (targetWallet == null)
                return GenericResult<WalletWithTransactionsDTO>.Error((int)HttpStatusCode.NotFound,
                                        "Wallet is not found.");

            var response = _mapper.Map<WalletWithTransactionsDTO>(targetWallet);

            return GenericResult<WalletWithTransactionsDTO>.Success(response);
        }
    }
}

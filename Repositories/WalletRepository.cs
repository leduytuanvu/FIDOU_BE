using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.DbContextVoiceAPI;
using VoiceAPI.Entities;
using VoiceAPI.IRepositories;
using VoiceAPI.Models.Responses.Wallets;
using VoiceAPI.Repository;

namespace VoiceAPI.Repositories
{
    public class WalletRepository : BaseRepository<Wallet>, IWalletRepository
    {
        private readonly VoiceAPIDbContext _context;

        public WalletRepository(VoiceAPIDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Wallet>> GetAllWalletWithTransactions()
        {
            var wallets = await _context.Wallets.AsNoTracking()
                                    .Include(tempWallet => tempWallet.TransactionHistories)
                                    .ToListAsync();

            return wallets;
        }

        public async Task<Wallet> GetWalletWithTransactions(Guid id)
        {
            var wallet = await _context.Wallets.AsNoTracking()
                                    .Include(tempWallet => tempWallet.TransactionHistories)
                                    .FirstOrDefaultAsync(tempWallet => tempWallet.Id.CompareTo(id) == 0);

            return wallet;
        }

        public async Task<Wallet> UpdateBalanceAfterAcceptInvitation(Guid walletId, decimal jobPrice)
        {
            var wallet = await GetById(walletId);

            var chargeAmount = decimal.Multiply(jobPrice, 0.1m); // 10% of JobPrice

            wallet.AvailableBalance = decimal.Subtract(wallet.AvailableBalance, chargeAmount);
            wallet.LockedBalance = decimal.Add(wallet.LockedBalance, chargeAmount);

            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }
        
        public async Task<Wallet> UpdateBalanceAfterRejectInvitation(Guid walletId, decimal jobPrice)
        {
            var wallet = await GetById(walletId);

            wallet.AvailableBalance = decimal.Add(wallet.AvailableBalance, jobPrice);
            wallet.LockedBalance = decimal.Subtract(wallet.LockedBalance, jobPrice);

            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }

        public async Task<Wallet> UpdateBalanceAfterInviteCandidateForWorking(Guid walletId, decimal jobPrice)
        {
            var response = await GetById(walletId);

            response.AvailableBalance = decimal.Subtract(response.AvailableBalance, jobPrice);
            response.LockedBalance = decimal.Add(response.LockedBalance, jobPrice);

            _context.Wallets.Update(response);
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<Wallet> UpdateBalanceAfterRejected(Guid walletId, decimal balance)
        {
            var response = await GetById(walletId);

            var refundAmount = decimal.Multiply(balance, 0.1m); // 10% of JobPrice

            response.AvailableBalance = decimal.Add(response.AvailableBalance, refundAmount);
            response.LockedBalance = decimal.Subtract(response.LockedBalance, refundAmount);

            _context.Wallets.Update(response);
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<Wallet> UpdateCandidateBalanceAfterFinishOrder(Guid walletId, decimal jobPrice)
        {
            var wallet = await GetById(walletId);

            var chargeAmount = decimal.Multiply(jobPrice, 0.1m); // 10% of JobPrice

            wallet.AvailableBalance = decimal.Add(wallet.AvailableBalance, jobPrice);
            wallet.LockedBalance = decimal.Subtract(wallet.LockedBalance, chargeAmount);

            _context.Wallets.Update(wallet);

            return wallet;
        }

        public async Task<Wallet> UpdateCandidateBalanceAfterReviewTrue(Guid walletId, decimal jobPrice)
        {
            var targetWallet = await GetById(walletId);

            var chargeAmount = decimal.Multiply(jobPrice, 0.1m); // 10% of JobPrice
            targetWallet.LockedBalance = decimal.Subtract(targetWallet.LockedBalance, chargeAmount);

            _context.Wallets.Update(targetWallet);
            await _context.SaveChangesAsync();

            return targetWallet;
        }

        public async Task<Wallet> GetWalletByDepositCode(string depositCode)
        {
            var targetWallet = await Get()
                .AsNoTracking()
                .FirstOrDefaultAsync(tempWallet => tempWallet.DepositCode.Equals(depositCode));

            return targetWallet;
        }

        public async Task<Wallet> UpdateBalanceAfterUpdateJob(Guid walletId, decimal amount, bool isCheaper)
        {
            var wallet = await GetById(walletId);

            if (isCheaper)
            {
                wallet.AvailableBalance = decimal.Add(wallet.AvailableBalance, amount);
                wallet.LockedBalance = decimal.Subtract(wallet.LockedBalance, amount);
            }
            else
            {
                wallet.AvailableBalance = decimal.Subtract(wallet.AvailableBalance, amount);
                wallet.LockedBalance = decimal.Add(wallet.LockedBalance, amount);
            }

            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }

        public async Task<Wallet> UpdateEnterpriseBalanceAfterDeleteJob(Guid walletId, decimal amount)
        {
            var wallet = await GetById(walletId);

            wallet.AvailableBalance = decimal.Add(wallet.AvailableBalance, amount);
            wallet.LockedBalance = decimal.Subtract(wallet.LockedBalance, amount);

            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }
        
        public async Task<List<Guid>> UpdateCandidatesBalanceAfterDeleteJob(List<Guid> walletIds, decimal amount)
        {
            List<Guid> successWalletIds = new();

            foreach (var walletId in walletIds)
            {
                if (successWalletIds.Contains(walletId))
                    continue;

                var wallet = await GetById(walletId);

                var chargedAmount = decimal.Multiply(amount, 0.1m);

                wallet.AvailableBalance = decimal.Add(wallet.AvailableBalance, chargedAmount);
                wallet.LockedBalance = decimal.Subtract(wallet.LockedBalance, chargedAmount);
                
                successWalletIds.Add(walletId);
            }
            
            return successWalletIds;
        }

        public async Task<Wallet> UpdateEnterpriseBalanceAfterFinishOrder(Guid walletId, decimal jobPrice)
        {
            var wallet = await GetById(walletId);

            wallet.LockedBalance = decimal.Subtract(wallet.LockedBalance, jobPrice);

            _context.Wallets.Update(wallet);

            return wallet;
        }

        public async Task<Wallet> UpdateEnterpriseBalanceAfterReviewTrue(Guid walletId, decimal jobPrice)
        {
            var targetWallet = await GetById(walletId);

            targetWallet.AvailableBalance = decimal.Add(targetWallet.AvailableBalance, jobPrice);
            targetWallet.LockedBalance = decimal.Subtract(targetWallet.LockedBalance, jobPrice);

            _context.Wallets.Update(targetWallet);
            await _context.SaveChangesAsync();

            return targetWallet;
        }
    }
}

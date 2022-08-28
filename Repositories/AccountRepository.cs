using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.DbContextVoiceAPI;
using VoiceAPI.Entities;
using VoiceAPI.IRepositories;
using VoiceAPI.Repository;

namespace VoiceAPI.Repositories
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        private readonly VoiceAPIDbContext _context;
        
        public AccountRepository(VoiceAPIDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Account> GetByEmail(string email)
        {
            var response = await Get().AsNoTracking()
                                        .FirstOrDefaultAsync(tempAccount => tempAccount.Email.Equals(email));

            return response;
        }

        public async Task<Enterprise> GetEnterpriseById(Guid id)
        {
            var targetEnterprise = await _context.Accounts.Join(_context.Enterprises,
                    account => account.Id,
                    enterprise => enterprise.Id,
                    (account, enterprise) => new { account, enterprise })
                .Where(result => result.enterprise.Id.CompareTo(id) == 0)
                .Select(result => result.enterprise)
                .FirstOrDefaultAsync();

            return targetEnterprise;
        }

        public async Task<Candidate> GetCandidateById(Guid id)
        {
            var targetCandidate = await _context.Accounts.Join(_context.Candidates,
                    account => account.Id,
                    candidate => candidate.Id,
                    (account, candidate) => new { account, candidate })
                .Where(result => result.candidate.Id.CompareTo(id) == 0)
                .Select(result => result.candidate)
                .FirstOrDefaultAsync();

            return targetCandidate;
        }

        public async Task<Account> GetByEmailAndPassword(string email, string password)
        {
            var account = await Get()
                .Where(tempAccount => tempAccount.Email.Equals(email))
                .FirstOrDefaultAsync();

            if (account == null)
                return null;

            var isVerified = BCrypt.Net.BCrypt.EnhancedVerify(password, account.Password);

            return isVerified 
                ? account 
                : null;
        }

        public async Task<Account> GetByPhoneNumberAndPassword(string phoneNumber, string password)
        {
            var account = await Get()
                .Where(tempAccount => tempAccount.PhoneNumber.Equals(phoneNumber))
                .FirstOrDefaultAsync();

            if (account == null)
                return null;

            var isVerified = BCrypt.Net.BCrypt.EnhancedVerify(password, account.Password);

            return isVerified
                ? account
                : null;
        }
    }
}

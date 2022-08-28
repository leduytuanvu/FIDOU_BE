using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.DbContextVoiceAPI;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;
using VoiceAPI.IRepositories;
using VoiceAPI.Repository;

namespace VoiceAPI.Repositories
{
    public class JobRepository : BaseRepository<Job>, IJobRepository
    {
        private readonly VoiceAPIDbContext _context;

        public JobRepository(VoiceAPIDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<decimal> GetAvailableBalanceOfEnterprise(Guid enterpriseId)
        {
            var wallet = await _context.Enterprises.Join(_context.Accounts,
                                            enterprise => enterprise.Id,
                                            account => account.Id,
                                            (enterprise, account) => new { enterprise = enterprise, account = account })
                                    .Where(result => result.enterprise.Id.CompareTo(enterpriseId) == 0)
                                    .Select(result => result.account).Join(_context.Wallets,
                                            account => account.Id,
                                            wallet => wallet.Id,
                                            (account, wallet) => new { account = account, wallet = wallet })
                                    .Select(result => result.wallet)
                                    .FirstOrDefaultAsync();
            return wallet != null
                    ? wallet.AvailableBalance
                    : -1;
        }

        public async Task<List<Candidate>> GetCandidatesAppliedForJob(Guid jobId)
        {
            var candidates = await _context.Jobs.Join(_context.Orders,
                    job => job.Id,
                    order => order.JobId,
                    (job, order) => new { job, order })
                .Where(result => result.job.Id.CompareTo(jobId) == 0)
                .Select(result => result.order).Join(_context.Candidates,
                    order => order.CandidateId,
                    candidate => candidate.Id,
                    (order, candidate) => new { order, candidate })
                .Select(result => result.candidate)
                .ToListAsync();

            return candidates;
        }

        public async Task<List<Order>> GetNotRejectedOrders(Guid jobId)
        {
            var orders = await _context.Jobs.Join(_context.Orders,
                    job => job.Id,
                    order => order.JobId,
                    (job, order) => new { job, order })
                .Where(result => result.job.Id.CompareTo(jobId) == 0
                                 && result.order.Status != OrderStatusEnum.REJECTED)
                .Select(result => result.order)
                .ToListAsync();

            return orders;
        }
    }
}

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
    public class EnterpriseRepository : BaseRepository<Enterprise>, IEnterpriseRepository
    {
        private readonly VoiceAPIDbContext _context;

        public EnterpriseRepository(VoiceAPIDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrderHistory(Guid id, OrderStatusEnum? orderStatus)
        {
            var response = await _context.Enterprises.Join(_context.Jobs,
                    enterprise => enterprise.Id,
                    job => job.EnterpriseId,
                    (enterprise, job) => new { enterprise, job })
                .Where(result => result.enterprise.Id.CompareTo(id) == 0)
                .Select(result => result.job).Join(_context.Orders,
                    job => job.Id,
                    order => order.JobId,
                    (job, order) => new { job, order })
                .Select(result => result.order)
                .ToListAsync();

            if (orderStatus != null)
            {
                response = response.AsEnumerable()
                    .Where(tempOrder => tempOrder.Status == orderStatus)
                    .ToList();
            }

            return response;
        }

        public async Task<List<Order>> GetOrdersOfJob(Guid jobId, OrderStatusEnum? orderStatus)
        {
            var response = await _context.Enterprises.Join(_context.Jobs,
                    enterprise => enterprise.Id,
                    job => job.EnterpriseId,
                    (enterprise, job) => new { enterprise, job })
                .Where(result => result.job.Id.CompareTo(jobId) == 0)
                .Select(result => result.job).Join(_context.Orders,
                    job => job.Id,
                    order => order.JobId,
                    (job, order) => new { job, order })
                .Select(result => result.order)
                .ToListAsync();

            if (orderStatus != null)
            {
                response = response.AsEnumerable()
                    .Where(tempOrder => tempOrder.Status == orderStatus)
                    .ToList();
            }

            return response;
        }

        public async Task<List<Job>> GetOwnJobs(Guid id)
        {
            var response = await _context.Enterprises.Join(_context.Jobs,
                    enterprise => enterprise.Id,
                    job => job.EnterpriseId,
                    (enterprise, job) => new { enterprise, job })
                .Where(result => result.enterprise.Id.CompareTo(id) == 0
                                 && result.job.JobStatus != JobStatusEnum.DELETED)
                .Select(result => result.job)
                .ToListAsync();

            return response;
        }

        public async Task<Account> UpdateAccountStatusAfterCreateProfile(Guid accountId)
        {
            var response = await _context.Enterprises.Join(_context.Accounts,
                    enterprise => enterprise.Id,
                    account => account.Id,
                    (enterprise, account) => new { enterprise, account })
                .Select(result => result.account)
                .FirstOrDefaultAsync();

            response.Status = Entities.Enums.AccountStatusEnum.ACTIVE;

            _context.Accounts.Update(response);
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<Candidate> GetCandidateByOrderId(Guid orderId)
        {
            var response = await _context.Enterprises.Join(_context.Jobs,
                    enterprise => enterprise.Id,
                    job => job.EnterpriseId,
                    (enterprise, job) => new { enterprise, job })
                .Select(result => result.job).Join(_context.Orders,
                    job => job.Id,
                    order => order.JobId,
                    (job, order) => new { job, order })
                .Where(result => result.order.Id.CompareTo(orderId) == 0)
                .Select(result => result.order).Join(_context.Candidates,
                    order => order.CandidateId,
                    candidate => candidate.Id,
                    (order, candidate) => new { order, candidate })
                .Select(result => result.candidate)
                .FirstOrDefaultAsync();

            return response;
        }

        public async Task<Job> GetJobByOrderId(Guid orderId)
        {
            var targetJob = await _context.Enterprises.Join(_context.Jobs,
                    enterprise => enterprise.Id,
                    job => job.EnterpriseId,
                    (enterprise, job) => new { enterprise, job })
                .Select(result => result.job).Join(_context.Orders,
                    job => job.Id,
                    order => order.JobId,
                    (job, order) => new { job, order })
                .Where(result => result.order.Id.CompareTo(orderId) == 0)
                .Select(result => result.job)
                .FirstOrDefaultAsync();

            return targetJob;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;
using VoiceAPI.IRepository;

namespace VoiceAPI.IRepositories
{
    public interface IEnterpriseRepository : IBaseRepository<Enterprise>
    {
        Task<Account> UpdateAccountStatusAfterCreateProfile(Guid accountId);
        Task<List<Job>> GetOwnJobs(Guid id);
        Task<List<Order>> GetOrderHistory(Guid id, OrderStatusEnum? orderStatus);
        Task<List<Order>> GetOrdersOfJob(Guid jobId, OrderStatusEnum? orderStatus);
        Task<Candidate> GetCandidateByOrderId(Guid orderId);
        Task<Job> GetJobByOrderId(Guid orderId);
    }
}

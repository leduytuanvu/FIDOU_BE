﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.IRepository;

namespace VoiceAPI.IRepositories
{
    public interface IJobRepository : IBaseRepository<Job>
    {
        Task<decimal> GetAvailableBalanceOfEnterprise(Guid enterpriseId);
        Task<List<Candidate>> GetCandidatesAppliedForJob(Guid jobId);
        Task<List<Order>> GetNotRejectedOrders(Guid jobId);
    }
}

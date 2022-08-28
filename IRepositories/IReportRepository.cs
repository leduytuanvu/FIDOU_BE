using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;
using VoiceAPI.IRepository;

namespace VoiceAPI.IRepositories
{
    public interface IReportRepository : IBaseRepository<Report>
    {
        Task<Candidate> GetCandidateByReportId(Guid id);
        Task<Job> GetJobByReportId(Guid id);
        Task<Job> UpdateJobStatus(Guid jobId, JobStatusEnum status);
        Task<Enterprise> GetEnterpriseByReportId(Guid id);
    }
}

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
    public class ReportRepository : BaseRepository<Report>, IReportRepository
    {
        public readonly VoiceAPIDbContext _context;

        public ReportRepository(VoiceAPIDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Candidate> GetCandidateByReportId(Guid id)
        {
            var targetCandidate = await _context.Reports.Join(_context.Orders,
                                                    report => report.Id,
                                                    order => order.Id,
                                                    (report, order) => new { report, order })
                                            .Where(result => result.report.Id.CompareTo(id) == 0)
                                            .Select(result => result.order).Join(_context.Candidates,
                                                    order => order.CandidateId,
                                                    candidate => candidate.Id,
                                                    (order, candidate) => new { order, candidate })
                                            .Select(result => result.candidate)
                                            .FirstOrDefaultAsync();

            return targetCandidate;
        }

        public async Task<Job> GetJobByReportId(Guid id)
        {
            var targetJob = await _context.Reports.Join(_context.Orders,
                                                    report => report.Id,
                                                    order => order.Id,
                                                    (report, order) => new { report, order })
                                            .Where(result => result.report.Id.CompareTo(id) == 0)
                                            .Select(result => result.order).Join(_context.Jobs,
                                                    order => order.JobId,
                                                    job => job.Id,
                                                    (order, job) => new { order, job })
                                            .Select(result => result.job)
                                            .FirstOrDefaultAsync();

            return targetJob;
        }

        public async Task<Job> UpdateJobStatus(Guid jobId, JobStatusEnum status)
        {
            var targetJob = await _context.Reports.Join(_context.Orders,
                                                    report => report.Id,
                                                    order => order.Id,
                                                    (report, order) => new { report, order })
                                            .Select(result => result.order).Join(_context.Jobs,
                                                    order => order.JobId,
                                                    job => job.Id,
                                                    (order, job) => new { order, job })
                                            .Where(result => result.job.Id.CompareTo(jobId) == 0)
                                            .Select(result => result.job)
                                            .FirstOrDefaultAsync();

            targetJob.JobStatus = status;

            _context.Jobs.Update(targetJob);
            await _context.SaveChangesAsync();

            return targetJob;
        }

        public async Task<Enterprise> GetEnterpriseByReportId(Guid id)
        {
            var targetEnterprise = await _context.Reports.Join(_context.Orders,
                    report => report.Id,
                    order => order.Id,
                    (report, order) => new { report, order })
                .Where(result => result.report.Id.CompareTo(id) == 0)
                .Select(result => result.order).Join(_context.Jobs,
                    order => order.JobId,
                    job => job.Id,
                    (order, job) => new { order, job })
                .Select(result => result.job).Join(_context.Enterprises,
                    job => job.EnterpriseId,
                    enterprise => enterprise.Id,
                    (job, enterprise) => new { job, enterprise })
                .Select(result => result.enterprise)
                .FirstOrDefaultAsync();

            return targetEnterprise;
        }
    }
}

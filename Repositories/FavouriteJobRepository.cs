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
    public class FavouriteJobRepository : BaseRepository<FavouriteJob>, IFavouriteJobRepository
    {
        private readonly VoiceAPIDbContext _context;

        public FavouriteJobRepository(VoiceAPIDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<FavouriteJob> GetByCandidateIdAndJobId(Guid candidateId, Guid jobId)
        {
            var targetFavouriteJob = await Get()
                .Where(tempFavouriteJob => tempFavouriteJob.CandidateId.CompareTo(candidateId) == 0
                                            && tempFavouriteJob.JobId.CompareTo(jobId) == 0)
                .FirstOrDefaultAsync();

            return targetFavouriteJob;
        }

        public async Task<Job> GetJobByFavouriteJobId(Guid id)
        {
            var targetJob = await _context.FavouriteJobs.Join(_context.Jobs,
                    favJob => favJob.JobId,
                    job => job.Id,
                    (favJob, job) => new { favJob, job })
                .Where(result => result.favJob.Id.CompareTo(id) == 0)
                .Select(result => result.job)
                .FirstOrDefaultAsync();

            return targetJob;
        }
    }
}

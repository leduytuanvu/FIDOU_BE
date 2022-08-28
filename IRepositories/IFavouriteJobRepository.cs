using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.IRepository;
using VoiceAPI.Models.Responses.Jobs;

namespace VoiceAPI.IRepositories
{
    public interface IFavouriteJobRepository : IBaseRepository<FavouriteJob>
    {
        Task<FavouriteJob> GetByCandidateIdAndJobId(Guid candidateId, Guid jobId);
        Task<Job> GetJobByFavouriteJobId(Guid id);
    }
}

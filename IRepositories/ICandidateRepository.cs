using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;
using VoiceAPI.IRepository;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Candidates;

namespace VoiceAPI.IRepositories
{
    public interface ICandidateRepository : IBaseRepository<Candidate>
    {
        Task<List<VoiceDemo>> GetVoiceDemosByCandidateId(Guid id);

        Task<Category> GetCategoryById(Guid? categoryId);

        Task<List<Order>> GetOrderHistory(Guid id, OrderStatusEnum? orderStatus);

        Task<Job> GetJobByOrderId(Guid orderId);
        Task<List<Review>> GetReviewsByCandidateId(Guid id);
        Task<GenericResult<CandidateReviewPointResultDataModel>> GetAveragePointOfCandidate(Guid id);
        Task<Candidate> UpdateCandidateStatusAfterReview(Candidate candidate);
    }
}

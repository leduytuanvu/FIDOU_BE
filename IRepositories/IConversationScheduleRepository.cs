using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.IRepository;

namespace VoiceAPI.IRepositories
{
    public interface IConversationScheduleRepository : IBaseRepository<ConversationSchedule>
    {
        Task<ConversationSchedule> GetByEnterpriseIdAndCandidateId(Guid enterpriseId, Guid candidateId);
        Task<ConversationSchedule> GetByOrderId(Guid orderId);
    }
}

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
    public class ConversationScheduleRepository : BaseRepository<ConversationSchedule>, IConversationScheduleRepository
    {
        public ConversationScheduleRepository(VoiceAPIDbContext context) : base(context)
        {
        }

        public async Task<ConversationSchedule> GetByEnterpriseIdAndCandidateId(Guid enterpriseId, Guid candidateId)
        {
            var response = await Get().AsNoTracking()
                                        .Where(tempConversationSchedule => tempConversationSchedule.EnterpriseId.CompareTo(enterpriseId) == 0
                                                            && tempConversationSchedule.CandidateId.CompareTo(candidateId) == 0
                                                            && DateTime.Compare(tempConversationSchedule.ScheduledTime, DateTime.UtcNow) > 0)
                                        .FirstOrDefaultAsync();

            return response;
        }

        public async Task<ConversationSchedule> GetByOrderId(Guid orderId)
        {
            var response = await Get().AsNoTracking()
                .Where(tempConversationSchedule => tempConversationSchedule.OrderId.CompareTo(orderId) == 0
                                                   && DateTime.Compare(tempConversationSchedule.ScheduledTime, DateTime.UtcNow) > 0)
                .FirstOrDefaultAsync();

            return response;
        }
    }
}

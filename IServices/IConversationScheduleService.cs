using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.ConversationSchedules;
using VoiceAPI.Models.Payload.ConversationSchedules;
using VoiceAPI.Models.Responses.ConversationSchedules;

namespace VoiceAPI.IServices
{
    public interface IConversationScheduleService
    {
        Task<GenericResult<ConversationScheduleDTO>> EnterpriseCreateConversationSchedule(ConversationScheduleCreateDataModel dataModel);
        Task<GenericResult<ConversationScheduleDTO>> GetByEnterpriseIdAndCandidateId(Guid enterpriseId, Guid candidateId);
        Task<GenericResult<ConversationScheduleDTO>> GetByOrderId(Guid orderId);
    }
}

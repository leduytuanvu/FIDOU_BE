using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Notifications;
using VoiceAPI.Models.Payload.Notifications;
using VoiceAPI.Models.Responses.Notifications;

namespace VoiceAPI.IServices
{
    public interface INotificationService
    {
        Task<GenericResult<NotifyBaseDTO>> PostNotification(NotificationPayload payload);

        Task<GenericResult<NotifyCandidateInvitedDTO>> PostNotifyCandidateInvited(
            NotifyCandidateInvitedDataModel dataModel);

        Task<GenericResult<NotifyJobHaveNewApplicantDTO>> PostNotifyJobHaveNewApplicant(
            NotifyJobHaveNewApplicantDataModel dataModel);

        Task<GenericResult<NotifyConversationScheduleDTO>> PostNotifyConversationScheduled(
            NotifyConversationScheduleDataModel dataModel);
        
        Task<GenericResult<bool>> SeenNotify(NotifyCandidateInvitedDataModel dataModel);
    }
}

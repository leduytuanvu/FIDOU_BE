using VoiceAPI.Models.Data.Notifications;

namespace VoiceAPI.Models.Responses.Notifications
{
    public class NotifyCandidateInvitedFetchDTO
    {
        public string Id { get; set; }
        public NotifyCandidateInvitedDataModel Data { get; set; }
    }
}
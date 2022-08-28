using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Data.Notifications;
using VoiceAPI.Models.Payload.Notifications;
using VoiceAPI.Models.Responses.Notifications;

namespace VoiceAPI.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotifyBaseDTO, NotifyBaseDataModel>().ReverseMap();
            CreateMap<NotificationPayload, NotifyBaseDataModel>().ReverseMap();

            CreateMap<NotifyCandidateInvitedDataModel, NotifyCandidateInvitedDTO>().ReverseMap();

            CreateMap<NotifyJobHaveNewApplicantDataModel, NotifyJobHaveNewApplicantDTO>().ReverseMap();

            CreateMap<NotifyConversationScheduleDataModel, NotifyConversationScheduleDTO>().ReverseMap();
        }
    }
}

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Models.Data.ConversationSchedules;
using VoiceAPI.Models.Payload.ConversationSchedules;
using VoiceAPI.Models.Responses.ConversationSchedules;

namespace VoiceAPI.Profiles
{
    public class ConversationScheduleProfile : Profile
    {
        public ConversationScheduleProfile()
        {
            CreateMap<ConversationScheduleCreatePayload, ConversationScheduleCreateDataModel>().ReverseMap();
            CreateMap<ConversationSchedule, ConversationScheduleCreateDataModel>().ReverseMap();

            CreateMap<ConversationSchedule, ConversationScheduleDTO>().ReverseMap();
        }
    }
}

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Models.Data.VoiceDemos;
using VoiceAPI.Models.Payload.VoiceDemos;
using VoiceAPI.Models.Responses.VoiceDemos;

namespace VoiceAPI.Profiles
{
    public class VoiceDemoProfile : Profile
    {
        public VoiceDemoProfile()
        {
            CreateMap<VoiceDemoCreatePayload, VoiceDemoCreateDataModel>().ReverseMap();

            CreateMap<VoiceDemo, VoiceDemoCreateDataModel>().ReverseMap();

            CreateMap<VoiceDemo, VoiceDemoDTO>().ReverseMap();
            
            CreateMap<VoiceDemoUpdateTextTranscriptPayload, VoiceDemoUpdateTextTranscriptDataModel>().ReverseMap();
            CreateMap<VoiceDemo, VoiceDemoUpdateTextTranscriptDataModel>().ReverseMap()
                .ForAllMembers(opts => opts.Condition(
                    (src, dest, srcMember) => srcMember != null)
                );
        }
    }
}

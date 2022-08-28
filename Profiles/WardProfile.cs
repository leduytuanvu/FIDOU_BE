using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Models.Payload.Wards;
using VoiceAPI.Models.Responses.Wards;

namespace VoiceAPI.Profiles
{
    public class WardProfile : Profile
    {
        public WardProfile()
        {
            CreateMap<Ward, WardDTO>().ReverseMap();

            CreateMap<Ward, WardCreatePayload>().ReverseMap();
        }
    }
}

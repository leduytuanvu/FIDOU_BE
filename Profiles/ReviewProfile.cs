using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Models.Payload.Reviews;
using VoiceAPI.Models.Responses.Reviews;

namespace VoiceAPI.Profiles
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewEnterpriseCreatePayload>().ReverseMap();

            CreateMap<Review, ReviewDTO>().ReverseMap();
        }
    }
}

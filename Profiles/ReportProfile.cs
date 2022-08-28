using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Models.Data.Reports;
using VoiceAPI.Models.Payload.Reports;
using VoiceAPI.Models.Responses.Reports;

namespace VoiceAPI.Profiles
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<Report, ReportCreatePayload>().ReverseMap();
            CreateMap<Report, ReportCreateDataModel>().ReverseMap();

            CreateMap<Report, ReportAdminReviewPayload>().ReverseMap();

            CreateMap<Report, ReportDTO>().ReverseMap();
        }
    }
}

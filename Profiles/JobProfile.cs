using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Models.Data.Jobs;
using VoiceAPI.Models.Payload.Jobs;
using VoiceAPI.Models.Responses.Jobs;

namespace VoiceAPI.Profiles
{
    public class JobProfile : Profile
    {
        public JobProfile()
        {
            CreateMap<Job, JobEnterprisePostJobPayload>().ReverseMap();
            CreateMap<Job, JobEnterprisePostJobDataModel>().ReverseMap();
            CreateMap<JobEnterprisePostJobPayload, JobEnterprisePostJobDataModel>().ReverseMap();

            CreateMap<Job, JobCandidateApplyJobPayload>().ReverseMap();

            CreateMap<Job, JobDTO>().ReverseMap();

            CreateMap<JobEnterpriseInviteCandidateForWorkingPayload, JobEnterpriseInviteCandidateForWorkingDataModel>().ReverseMap();
            CreateMap<Job, JobEnterpriseInviteCandidateForWorkingDataModel>().ReverseMap();
            CreateMap<Job, JobWithInvitationDTO>().ReverseMap();

            CreateMap<Job, JobWithOrdersDTO>().ReverseMap();

            CreateMap<JobUpdatePayload, JobUpdateDataModel>().ReverseMap();
            CreateMap<Job, JobUpdateDataModel>().ReverseMap()
                .ForAllMembers(opts => opts.Condition(
                    (src, dest, srcMember) => srcMember != null)
                );

            CreateMap<Job, JobDeleteDataModel>().ReverseMap();
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.IRepositories;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.FavouriteJobs;
using VoiceAPI.Models.Responses.FavouriteJobs;
using VoiceAPI.Models.Responses.Jobs;

namespace VoiceAPI.Services
{
    public class FavouriteJobService : IFavouriteJobService
    {
        private readonly IMapper _mapper;

        private readonly IFavouriteJobRepository _favouriteJobRepository;
        
        private readonly IJobService _jobService;

        public FavouriteJobService(IMapper mapper, 
            IFavouriteJobRepository favouriteJobRepository, 
            IJobService jobService)
        {
            _mapper = mapper;

            _favouriteJobRepository = favouriteJobRepository;

            _jobService = jobService;
        }

        public async Task<GenericResult<FavouriteJobCreateDTO>> CreateNew(FavouriteJobCreateDataModel dataModel)
        {
            var targetFavouriteJob = 
                await _favouriteJobRepository.GetByCandidateIdAndJobId(dataModel.CandidateId, dataModel.JobId);

            if (targetFavouriteJob != null)
                return GenericResult<FavouriteJobCreateDTO>.Error((int)HttpStatusCode.Forbidden,
                                        "V400_27",
                                        "Công việc này đã được thêm vào mục yêu thích.");

            targetFavouriteJob = _mapper.Map<FavouriteJob>(dataModel);

            _favouriteJobRepository.Create(targetFavouriteJob);
            await _favouriteJobRepository.SaveAsync();

            var response = _mapper.Map<FavouriteJobCreateDTO>(targetFavouriteJob);

            return GenericResult<FavouriteJobCreateDTO>.Success(response);
        }

        public async Task<GenericResult<List<FavouriteJobDTO>>> GetAll()
        {
            var targetFavourite 
                = await _favouriteJobRepository.Get().ToListAsync();

            var response = _mapper.Map<List<FavouriteJobDTO>>(targetFavourite);

            foreach (var temp in response)
            {
                var targetJob = await _favouriteJobRepository.GetJobByFavouriteJobId(temp.Id);
                temp.Job = _mapper.Map<JobDTO>(targetJob);
            }

            return GenericResult<List<FavouriteJobDTO>>.Success(response);
        }

        public async Task<GenericResult<List<FavouriteJobDTO>>> GetAllByCandidateId(Guid candidateId)
        {
            var targetFavouriteJob = await _favouriteJobRepository.Get()
                                        .Where(tempFavouriteJob => tempFavouriteJob.CandidateId.CompareTo(candidateId) == 0)
                                        .ToListAsync();

            var response = _mapper.Map<List<FavouriteJobDTO>>(targetFavouriteJob);

            foreach (var temp in response)
            {
                var targetJob = await _favouriteJobRepository.GetJobByFavouriteJobId(temp.Id);
                temp.Job = _mapper.Map<JobDTO>(targetJob);
            }
            
            return GenericResult<List<FavouriteJobDTO>>.Success(response);
        }

        public async Task<GenericResult<FavouriteJobDTO>> GetById(Guid id)
        {
            var targetFavouriteJob = await _favouriteJobRepository.GetById(id);

            if (targetFavouriteJob == null)
                return GenericResult<FavouriteJobDTO>.Error((int)HttpStatusCode.NotFound,
                                        "Favourite Job is not found.");

            var response = _mapper.Map<FavouriteJobDTO>(targetFavouriteJob);
            response.Job = (await _jobService.GetById(targetFavouriteJob.JobId)).Data;

            return GenericResult<FavouriteJobDTO>.Success(response);
        }

        public async Task<GenericResult<FavouriteJobDTO>> Remove(FavouriteJobRemoveDataModel dataModel)
        {
            var targetFavouriteJob =
                await _favouriteJobRepository.GetByCandidateIdAndJobId(dataModel.CandidateId, dataModel.JobId);

            if (targetFavouriteJob == null)
                return GenericResult<FavouriteJobDTO>.Error((int)HttpStatusCode.NotFound,
                                        "FavouriteJob is not found.");

            _favouriteJobRepository.Delete(targetFavouriteJob);
            await _favouriteJobRepository.SaveAsync();

            var response = _mapper.Map<FavouriteJobDTO>(targetFavouriteJob);

            return GenericResult<FavouriteJobDTO>.Success(response);
        }
    }
}

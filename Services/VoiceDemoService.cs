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
using VoiceAPI.Models.Data.VoiceDemos;
using VoiceAPI.Models.Responses.VoiceDemos;

namespace VoiceAPI.Services
{
    public class VoiceDemoService : IVoiceDemoService
    {
        private readonly IMapper _mapper;

        private readonly IVoiceDemoRepository _voiceDemoRepository;

        private readonly ICandidateService _candidateService;
        private readonly ISubCategoryService _subCategoryService;


        public VoiceDemoService(IMapper mapper, 
            IVoiceDemoRepository voiceDemoRepository, 
            ICandidateService candidateService, 
            ISubCategoryService subCategoryService)
        {
            _mapper = mapper;

            _voiceDemoRepository = voiceDemoRepository;

            _candidateService = candidateService;
            _subCategoryService = subCategoryService;
        }

        public async Task<GenericResult<VoiceDemoDTO>> CreateNew(VoiceDemoCreateDataModel dataModel)
        {
            var targetVoiceDemo = _mapper.Map<VoiceDemo>(dataModel);

            var targetCandidate = await _candidateService.GetById(dataModel.CandidateId);

            if (targetCandidate.Data == null)
                return GenericResult<VoiceDemoDTO>.Error((int)HttpStatusCode.NotFound,
                                        "Candidate is not found.");

            var targetSubCategory = await _subCategoryService.GetById(dataModel.SubCategoryId);

            if (targetSubCategory.Data == null)
                return GenericResult<VoiceDemoDTO>.Error((int)HttpStatusCode.NotFound,
                                        "SubCategory is not found.");

            _voiceDemoRepository.Create(targetVoiceDemo);
            await _voiceDemoRepository.SaveAsync();

            var response = _mapper.Map<VoiceDemoDTO>(targetVoiceDemo);
            response.SubCategoryName = targetSubCategory.Data.Name;

            return GenericResult<VoiceDemoDTO>.Success(response);
        }

        public async Task<GenericResult<List<VoiceDemoDTO>>> GetAllByCandidateId(Guid candidateId)
        {
            var targetCandidate = await _candidateService.GetById(candidateId);

            if (targetCandidate.Data == null)
                return GenericResult<List<VoiceDemoDTO>>.Error((int)HttpStatusCode.NotFound,
                                            "Candidate is not found.");

            var targetVoiceDemos = await _voiceDemoRepository.Get()
                                                    .AsNoTracking()
                                                    .ToListAsync();

            var subCategoryIds = targetVoiceDemos
                                        .AsEnumerable()
                                        .Select(tempVoiceDemo => tempVoiceDemo.SubCategoryId)
                                        .ToList();

            var response = _mapper.Map<List<VoiceDemoDTO>>(targetVoiceDemos);

            for (var i = 0; i < response.Count; i++)
            {
                var targetSubCategory = await _subCategoryService.GetById(subCategoryIds[i]);

                response[i].SubCategoryName = targetSubCategory.Data.Name;
            }

            return GenericResult<List<VoiceDemoDTO>>.Success(response);
        }
        
        public async Task<GenericResult<VoiceDemoDTO>> UpdateTextTranscript(VoiceDemoUpdateTextTranscriptDataModel dataModel)
        {
            var targetVoiceDemo = await _voiceDemoRepository.GetById(dataModel.Id);
            
            if (targetVoiceDemo == null) 
                return GenericResult<VoiceDemoDTO>.Error((int)HttpStatusCode.NotFound, 
                    "Voice demo is not found.");
            
            if (targetVoiceDemo.TextTranscript != null) 
                return GenericResult<VoiceDemoDTO>.Error((int)HttpStatusCode.BadRequest, 
                    "V400_51", 
                    "Bản ghi văn bản đã được cập nhật.");

            var targetCandidate = await _candidateService.GetById(dataModel.CandidateId);
            
            if (targetVoiceDemo.CandidateId.CompareTo(targetCandidate.Data.Id) != 0) 
                return GenericResult<VoiceDemoDTO>.Error((int)HttpStatusCode.BadRequest, 
                    "V400_50", 
                    "Bản thử giọng nói không phải của ứng viên.");

            _mapper.Map(dataModel, targetVoiceDemo);

            _voiceDemoRepository.Update(targetVoiceDemo);
            await _voiceDemoRepository.SaveAsync();

            var response = _mapper.Map<VoiceDemoDTO>(targetVoiceDemo);

            return GenericResult<VoiceDemoDTO>.Success(response);
        }
    }
}

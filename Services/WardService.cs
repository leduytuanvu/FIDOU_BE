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
using VoiceAPI.Models.Payload.Wards;
using VoiceAPI.Models.Responses.Wards;

namespace VoiceAPI.Services
{
    public class WardService : IWardService
    {
        private readonly IMapper _mapper;

        private readonly IWardRepository _wardRepository;

        private readonly IDistrictService _districtService;

        public WardService(IMapper mapper, 
            IWardRepository wardRepository, 
            IDistrictService districtService)
        {
            _mapper = mapper;

            _wardRepository = wardRepository;

            _districtService = districtService;
        }

        public async Task<GenericResult<List<WardDTO>>> CreateAllNew(List<WardCreatePayload> payloads)
        {
            List<WardDTO> response = new();

            var isCodeDuplicate = payloads.GroupBy(tempDistrict => tempDistrict.Code)
                                            .Any(tempDistrict => tempDistrict.Count() > 1);

            var isNameDuplicate = payloads.GroupBy(tempDistrict => tempDistrict.Name)
                                            .Any(tempDistrict => tempDistrict.Count() > 1);

            if (isCodeDuplicate || isNameDuplicate)
                return GenericResult<List<WardDTO>>.Error((int)HttpStatusCode.BadRequest,
                                            "V400_35",
                                            "Tên và mã khu vực đã tồn tại.");

            await _wardRepository.RemoveAll();

            foreach (var payload in payloads)
            {
                var targetDistrict = await _districtService.GetById(payload.DistrictCode);

                if (targetDistrict.Data == null)
                    return GenericResult<List<WardDTO>>.Error((int)HttpStatusCode.NotFound,
                                            "District is not found.");

                var targetWard = _mapper.Map<Ward>(payload);

                _wardRepository.Create(targetWard);

                response.Add(_mapper.Map<WardDTO>(targetWard));
            }

            await _wardRepository.SaveAsync();

            return GenericResult<List<WardDTO>>.Success(response);
        }

        public async Task<GenericResult<List<WardDTO>>> GetAll()
        {
            var wards = await _wardRepository.Get().AsNoTracking().ToListAsync();

            var response = _mapper.Map<List<WardDTO>>(wards);

            foreach (var tempWard in response)
            {
                var targetDistrict = await _districtService.GetById(tempWard.DistrictCode);
                tempWard.DistrictName = targetDistrict?.Data?.Name;
            }

            return GenericResult<List<WardDTO>>.Success(response);
        }

        public async Task<GenericResult<WardDTO>> GetById(string code)
        {
            var ward = await _wardRepository.GetById(code);

            var response = _mapper.Map<WardDTO>(ward);

            return GenericResult<WardDTO>.Success(response);
        }

        public async Task<GenericResult<WardDTO>> GetByName(string name)
        {
            var targetWard = await _wardRepository
                .Get()
                .AsNoTracking()
                .FirstOrDefaultAsync(tempWard => tempWard.Name.Equals(name));

            var response = _mapper.Map<WardDTO>(targetWard);

            return GenericResult<WardDTO>.Success(response);
        }
    }
}

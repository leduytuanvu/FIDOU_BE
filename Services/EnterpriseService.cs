using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.IRepositories;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Enterprises;
using VoiceAPI.Models.Data.Orders;
using VoiceAPI.Models.Payload.Enterprises;
using VoiceAPI.Models.Responses.Candidates;
using VoiceAPI.Models.Responses.Enterprises;
using VoiceAPI.Models.Responses.Jobs;
using VoiceAPI.Models.Responses.Orders;

namespace VoiceAPI.Services
{
    public class EnterpriseService : IEnterpriseService
    {
        private readonly IMapper _mapper;

        private readonly IEnterpriseRepository _enterpriseRepository;

        private readonly IAccountService _accountService;
        private readonly IProvinceService _provinceService;
        private readonly IDistrictService _districtService;
        private readonly IWardService _wardService;

        public EnterpriseService(IMapper mapper, 
            IEnterpriseRepository enterpriseRepository, 
            IAccountService accountService, 
            IProvinceService provinceService, 
            IDistrictService districtService, 
            IWardService wardService)
        {
            _mapper = mapper;

            _enterpriseRepository = enterpriseRepository;
            _accountService = accountService;
            _provinceService = provinceService;
            _districtService = districtService;
            _wardService = wardService;
        }

        public async Task<GenericResult<EnterpriseDTO>> CreateNew(EnterpriseCreateDataModel dataModel)
        {
            var targetEnterprise = _mapper.Map<Enterprise>(dataModel);

            var targetProvince = await _provinceService.GetById(dataModel.ProvinceCode);

            if (targetProvince.Data == null)
                return GenericResult<EnterpriseDTO>.Error((int)HttpStatusCode.NotFound,
                                            "Province is not found.");

            var targetDistrict = await _districtService.GetById(dataModel.DistrictCode);

            if (targetDistrict.Data == null)
                return GenericResult<EnterpriseDTO>.Error((int)HttpStatusCode.NotFound,
                                            "District is not found.");

            var targetWard = await _wardService.GetById(dataModel.WardCode);

            if (targetWard.Data == null)
                return GenericResult<EnterpriseDTO>.Error((int)HttpStatusCode.NotFound,
                                            "Ward is not found.");

            targetEnterprise.Province = targetProvince.Data.Code;
            targetEnterprise.District = targetDistrict.Data.Code;
            targetEnterprise.Ward = targetWard.Data.Code;

            _enterpriseRepository.Create(targetEnterprise);
            await _enterpriseRepository.SaveAsync();

            await _enterpriseRepository.UpdateAccountStatusAfterCreateProfile(targetEnterprise.Id);

            var targetAccount = await _accountService.GetById(targetEnterprise.Id);

            await _accountService.UpdateAccountStatus(targetAccount.Data.Id, Entities.Enums.AccountStatusEnum.ACTIVE);

            var response = _mapper.Map<EnterpriseDTO>(targetEnterprise);

            return GenericResult<EnterpriseDTO>.Success(response);
        }

        public async Task<GenericResult<EnterpriseWithJobsDTO>> GetById(Guid id)
        {
            var targetEnterprise = await _enterpriseRepository.GetById(id);
            
            if (targetEnterprise == null) 
                return GenericResult<EnterpriseWithJobsDTO>.Error((int)HttpStatusCode.NotFound, 
                    "Enterprise is not found.");

            var ownJobs = await _enterpriseRepository.GetOwnJobs(id);

            var response = _mapper.Map<EnterpriseWithJobsDTO>(targetEnterprise);
            response.Jobs = _mapper.Map<List<JobDTO>>(ownJobs);

            var targetProvince = await _provinceService.GetById(targetEnterprise.Province);
            response.ProvinceCode = targetProvince?.Data?.Code;

            var targetDistrict = await _districtService.GetById(targetEnterprise.District);
            response.DistrictCode = targetDistrict?.Data?.Code;

            var targetWard = await _wardService.GetById(targetEnterprise.Ward);
            response.WardCode = targetWard?.Data?.Code;

            return GenericResult<EnterpriseWithJobsDTO>.Success(response);
        }

        public async Task<GenericResult<List<JobWithOrdersDTO>>> GetOrderHistory(OrderEnterpriseGetHistoryDataModel dataModel)
        {
            var targetJobs = await _enterpriseRepository.GetOwnJobs(dataModel.EnterpriseID);

            var response = _mapper.Map<List<JobWithOrdersDTO>>(targetJobs);

            for (var i = 0; i < targetJobs.Count; i++)
            {
                var targetOrders = await _enterpriseRepository.GetOrdersOfJob(targetJobs[i].Id, dataModel.Payload.Status);

                response[i].Orders = _mapper.Map<List<OrderHistoryDTO>>(targetOrders);

                foreach (var tempOrder in response[i].Orders)
                {
                    var targetCandidate = await _enterpriseRepository.GetCandidateByOrderId(tempOrder.Id);

                    tempOrder.Candidate = _mapper.Map<CandidateDTO>(targetCandidate);

                    var targetJob = await _enterpriseRepository.GetJobByOrderId(tempOrder.Id);

                    tempOrder.Job = _mapper.Map<JobDTO>(targetJob);
                }
            }

            return GenericResult<List<JobWithOrdersDTO>>.Success(response);
        }

        public async Task<GenericResult<EnterpriseDTO>> UpdateProfile(EnterpriseUpdateDataModel dataModel)
        {
            var targetEnterprise = await _enterpriseRepository.GetById(dataModel.Id);

            if (targetEnterprise == null)
                return GenericResult<EnterpriseDTO>.Error((int)HttpStatusCode.NotFound,
                    "Enterprise is not existed.");

            var targetProvince = await _provinceService.GetById(dataModel.ProvinceCode);

            if (targetProvince.Data == null)
                return GenericResult<EnterpriseDTO>.Error((int)HttpStatusCode.NotFound,
                    "Province is not found.");

            var targetDistrict = await _districtService.GetById(dataModel.DistrictCode);

            if (targetDistrict.Data == null)
                return GenericResult<EnterpriseDTO>.Error((int)HttpStatusCode.NotFound,
                    "District is not found.");

            var targetWard = await _wardService.GetById(dataModel.WardCode);

            if (targetWard.Data == null)
                return GenericResult<EnterpriseDTO>.Error((int)HttpStatusCode.NotFound,
                    "Ward is not found.");

            targetEnterprise.Province = targetProvince.Data.Code;
            targetEnterprise.District = targetDistrict.Data.Code;
            targetEnterprise.Ward = targetWard.Data.Code;
            
            _mapper.Map(dataModel, targetEnterprise);

            _enterpriseRepository.Update(targetEnterprise);
            await _enterpriseRepository.SaveAsync();

            var response = _mapper.Map<EnterpriseDTO>(targetEnterprise);

            return GenericResult<EnterpriseDTO>.Success(response);
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;
using VoiceAPI.IRepositories;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Candidates;
using VoiceAPI.Models.Data.Orders;
using VoiceAPI.Models.Payload.Candidates;
using VoiceAPI.Models.Responses.Candidates;
using VoiceAPI.Models.Responses.Jobs;
using VoiceAPI.Models.Responses.Orders;
using VoiceAPI.Models.Responses.Reviews;
using VoiceAPI.Models.Responses.SubCategories;
using VoiceAPI.Models.Responses.VoiceDemos;

namespace VoiceAPI.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly IMapper _mapper;

        private readonly ICandidateRepository _candidateRepository;

        private readonly IAccountService _accountService;
        private readonly IProvinceService _provinceService;
        private readonly ISubCategoryService _subCategoryService;

        public CandidateService(IMapper mapper,
            ICandidateRepository candidateRepository,
            IAccountService accountService,
            IProvinceService provinceService,
            ISubCategoryService subCategoryService)
        {
            _mapper = mapper;

            _candidateRepository = candidateRepository;
            _accountService = accountService;
            _provinceService = provinceService;
            _subCategoryService = subCategoryService;
        }

        public async Task<GenericResult<CandidateDTO>> CreateNew(CandidateCreateDataModel dataModel)
        {
            var targetCandidate = await _candidateRepository.GetById(dataModel.Id);

            if (targetCandidate != null)
                return GenericResult<CandidateDTO>.Error("V400_08",
                    "Ứng viên này đã tồn tại.");

            var targetAccount = await _accountService.GetById(dataModel.Id);

            if (targetAccount == null)
                return GenericResult<CandidateDTO>.Error("V400_09",
                    "Không thể tạo hồ sơ ứng viên cho tài khoản không tồn tại.");

            if (targetAccount.Data.Role != Entities.Enums.RoleEnum.CANDIDATE)
                return GenericResult<CandidateDTO>.Error("V400_08",
                    "Không thể tạo hồ sơ ứng viên cho tài khoản doanh nghiệp.");

            targetCandidate = _mapper.Map<Candidate>(dataModel);

            List<string> subCategoryNames = new();
            List<SubCategoryDTO> subCategories = new();

            foreach (var subCategoryId in dataModel.SubCategoryIds)
            {
                var tempSubCategory = await _subCategoryService.GetById(Guid.Parse(subCategoryId));

                if (tempSubCategory.Data == null)
                    return GenericResult<CandidateDTO>.Error((int)HttpStatusCode.NotFound,
                        "SubCategory is not found.");

                subCategoryNames.Add(tempSubCategory.Data.Name);
                subCategories.Add(tempSubCategory.Data);
            }

            targetCandidate.SubCategorieNames = subCategoryNames;

            var targetProvince = await _provinceService.GetById(dataModel.ProvinceCode);

            if (targetProvince.Data == null)
                return GenericResult<CandidateDTO>.Error((int)HttpStatusCode.NotFound,
                    "Province is not found.");

            targetCandidate.Province = targetProvince.Data.Name;

            _candidateRepository.Create(targetCandidate);
            await _candidateRepository.SaveAsync();

            await _accountService.UpdateAccountStatus(targetAccount.Data.Id, AccountStatusEnum.ACTIVE);

            var response = _mapper.Map<CandidateDTO>(targetCandidate);
            response.SubCategorieNames = subCategoryNames;
            response.ProvinceCode = targetProvince.Data.Code;

            return GenericResult<CandidateDTO>.Success(response);
        }

        public async Task<GenericResult<CandidateDTO>> GetById(Guid id)
        {
            var targetCandidate = await _candidateRepository.GetById(id);

            if (targetCandidate == null)
                return GenericResult<CandidateDTO>.Error((int)HttpStatusCode.NotFound,
                    "Candidate is not found.");

            var targetVoiceDemos = await _candidateRepository.GetVoiceDemosByCandidateId(id);

            var response = _mapper.Map<CandidateDTO>(targetCandidate);
            response.VoiceDemos = _mapper.Map<List<VoiceDemoDTO>>(targetVoiceDemos);

            for (var i = 0; i < targetVoiceDemos.Count; i++)
            {
                var targetSubCategory = await _subCategoryService.GetById(targetVoiceDemos[i].SubCategoryId);

                response.VoiceDemos[i].SubCategoryName = targetSubCategory.Data.Name;
            }

            var targetProvince = await _provinceService.GetByName(targetCandidate.Province);

            response.ProvinceCode = targetProvince.Data != null
                ? targetProvince.Data.Code
                : null;

            return GenericResult<CandidateDTO>.Success(response);
        }

        public async Task<GenericResult<CandidateGetProfileDTO>> GetProfile(Guid id)
        {
            var targetCandidate = await _candidateRepository.GetById(id);

            if (targetCandidate == null)
                return GenericResult<CandidateGetProfileDTO>.Error((int)HttpStatusCode.NotFound,
                    "Candidate is not found.");

            var targetVoiceDemos = await _candidateRepository.GetVoiceDemosByCandidateId(id);

            var response = _mapper.Map<CandidateGetProfileDTO>(targetCandidate);
            response.VoiceDemos = _mapper.Map<List<VoiceDemoDTO>>(targetVoiceDemos);

            for (var i = 0; i < targetVoiceDemos.Count; i++)
            {
                var targetSubCategory = await _subCategoryService.GetById(targetVoiceDemos[i].SubCategoryId);

                response.VoiceDemos[i].SubCategoryName = targetSubCategory.Data.Name;
            }

            var targetProvince = await _provinceService.GetByName(targetCandidate.Province);

            response.ProvinceName = targetProvince.Data != null
                ? targetProvince.Data.Name
                : null;

            var targetReviews = await _candidateRepository.GetReviewsByCandidateId(id);

            response.Reviews = _mapper.Map<List<ReviewDTO>>(targetReviews);

            return GenericResult<CandidateGetProfileDTO>.Success(response);
        }

        public async Task<GenericResult<List<OrderHistoryDTO>>> GetOrderHistory(
            OrderCandidateGetHistoryDataModel dataModel)
        {
            var targetOrders =
                await _candidateRepository.GetOrderHistory(dataModel.CandidateId, dataModel.Payload.Status);

            var response = _mapper.Map<List<OrderHistoryDTO>>(targetOrders);

            foreach (var temp in response)
            {
                var tempJob = await _candidateRepository.GetJobByOrderId(temp.Id);
                temp.Job = _mapper.Map<JobDTO>(tempJob);

                var targetCandidate = (await GetById(dataModel.CandidateId)).Data;
                temp.Candidate = targetCandidate;
            }

            return GenericResult<List<OrderHistoryDTO>>.Success(response);
        }

        public async Task<GenericResult<List<CandidateGetProfileDTO>>> GetWithSortingReviewPoint(
            CandidateGetWithSortingReviewPointPayload payload)
        {
            var candidates = await _candidateRepository.Get()
                .AsNoTracking()
                .ToListAsync();

            List<CandidateReviewPointResultDataModel> candidateReviewPointResults = new();
            foreach (var tempCandidate in candidates)
            {
                var candidateReviewPointResult =
                    await _candidateRepository.GetAveragePointOfCandidate(tempCandidate.Id);

                if (candidateReviewPointResult != null)
                    candidateReviewPointResults.Add(candidateReviewPointResult.Data);
            }

            List<CandidateGetProfileDTO> response = null;

            if (candidateReviewPointResults.Count > 0)
            {
                var sortedCandidateReviewPointResults
                    = candidateReviewPointResults.OrderByDescending(tempResult => tempResult.ReviewPointAverage).ToList();

                var sortedCandidates = sortedCandidateReviewPointResults
                    .Select(tempSortedResult => tempSortedResult.Candidate)
                    .ToList();

                var targetSortedCandidates = sortedCandidates
                    .Skip(payload.pageSize * (payload.pageNumber - 1))
                    .Take(payload.pageSize)
                    .ToList();
                
                var pointSorted = sortedCandidateReviewPointResults
                    .Skip(payload.pageSize * (payload.pageNumber - 1))
                    .Take(payload.pageSize)
                    .ToList();

                response = _mapper.Map<List<CandidateGetProfileDTO>>(targetSortedCandidates);

                foreach (var tempCandidate in response)
                {
                    var targetVoiceDemos = await _candidateRepository.GetVoiceDemosByCandidateId(tempCandidate.Id);
                    tempCandidate.VoiceDemos = _mapper.Map<List<VoiceDemoDTO>>(targetVoiceDemos);
                }

                for (int i = 0; i < response.Count; i++)
                {
                    response[i].ProvinceName = targetSortedCandidates[i].Province;
                    response[i].AverageReviewPoint = pointSorted[i].ReviewPointAverage;
                    
                    var targetVoiceDemos = await _candidateRepository.GetVoiceDemosByCandidateId(response[i].Id);
                    response[i].VoiceDemos = _mapper.Map<List<VoiceDemoDTO>>(targetVoiceDemos);
                    
                    for (int j = 0; j < targetVoiceDemos.Count; j++)
                    {
                        var targetSubcategory = await _subCategoryService.GetById(targetVoiceDemos[j].SubCategoryId);
                        response[i].VoiceDemos[j].SubCategoryName = targetSubcategory.Data.Name;
                    }
                }
            }

            if (response == null)
            {
                var targetCandidates = candidates
                    .Skip(payload.pageSize * (payload.pageNumber - 1))
                    .Take(payload.pageSize)
                    .ToList();
                response = _mapper.Map<List<CandidateGetProfileDTO>>(targetCandidates);
            }

            return GenericResult<List<CandidateGetProfileDTO>>.Success(response);
        }

        public async Task<GenericResult<List<CandidateGetProfileDTO>>> SearchFilter(
            CandidateSearchFilterPayload payload)
        {
            var targetCandidates = await _candidateRepository.Get()
                .AsNoTracking()
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(payload.SearchText))
            {
                targetCandidates = targetCandidates
                    .AsEnumerable()
                    .Where(tempCandidate => tempCandidate.Name.ToLower().Contains(payload.SearchText))
                    .ToList();
            }

            if (payload.CategoryId != null)
            {
                var targetCategory = await _subCategoryService.GetCategoryById(payload.CategoryId ?? Guid.Empty);

                if (targetCategory.Data == null)
                    return GenericResult<List<CandidateGetProfileDTO>>.Error((int)HttpStatusCode.NotFound,
                        "Category is not found.");

                if (payload.SubCategoryId != null)
                {
                    var targetSubCategory = await _subCategoryService.GetById(payload.SubCategoryId ?? Guid.Empty);

                    if (targetSubCategory.Data == null)
                        return GenericResult<List<CandidateGetProfileDTO>>.Error((int)HttpStatusCode.NotFound,
                            "Sub Category is not found.");

                    if (targetSubCategory.Data.CategoryId.CompareTo(payload.CategoryId) != 0)
                        return GenericResult<List<CandidateGetProfileDTO>>.Error((int)HttpStatusCode.BadRequest,
                            "V400_49",
                            "Danh mục phụ không thuộc về danh mục này");

                    targetCandidates = targetCandidates
                        .AsEnumerable()
                        .Where(tempCandidate => tempCandidate.SubCategorieNames.Contains(targetSubCategory.Data.Name))
                        .ToList();
                }
                else
                {
                    var tempSubCategories = await _subCategoryService.GetAllByCategoryId(targetCategory.Data.Id);
                    var targetSubCategoryNames = tempSubCategories.Data
                        .AsEnumerable()
                        .Select(temp => temp.Name)
                        .ToList();

                    for (var i = 0; i < targetCandidates.Count; i++)
                    {
                        var shouldRemove = true;

                        var tempSubCategoryNames = targetCandidates[i].SubCategorieNames;

                        foreach (var tempSubCategoryName in tempSubCategoryNames)
                        {
                            if (targetSubCategoryNames.Contains(tempSubCategoryName))
                            {
                                shouldRemove = false;
                            }
                        }

                        if (tempSubCategoryNames.Count == 0)
                            shouldRemove = true;

                        if (shouldRemove)
                        {
                            targetCandidates.RemoveAt(i);
                            i--;
                        }
                    }
                }

                if (payload.Gender != null)
                {
                    targetCandidates = targetCandidates
                        .AsEnumerable()
                        .Where(tempCandidate => tempCandidate.Gender.Equals(payload.Gender))
                        .ToList();
                }
            }

            var response = _mapper.Map<List<CandidateGetProfileDTO>>(targetCandidates);

            for (var i = 0; i < targetCandidates.Count; i++)
            {
                var targetVoiceDemos = await _candidateRepository.GetVoiceDemosByCandidateId(targetCandidates[i].Id);
                response[i].VoiceDemos = _mapper.Map<List<VoiceDemoDTO>>(targetVoiceDemos);

                var targetProvince = await _provinceService.GetByName(targetCandidates[i].Province);

                response[i].ProvinceName = targetProvince.Data?.Name;
            }

            return GenericResult<List<CandidateGetProfileDTO>>.Success(response);
        }

        public async Task<GenericResult<CandidateDTO>> UpdateProfile(CandidateUpdateDataModel dataModel)
        {
            var targetCandidate = await _candidateRepository.GetById(dataModel.Id);

            if (targetCandidate == null)
                return GenericResult<CandidateDTO>.Error((int)HttpStatusCode.NotFound,
                    "Candidate is not found.");

            if (dataModel.DOB != null && DateTime.Compare(dataModel.DOB ?? DateTime.Now, DateTime.Now) >= 0)
                return GenericResult<CandidateDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_55",
                    "Ngày sinh phải lớn hơn ngày hiện tại");

            _mapper.Map(dataModel, targetCandidate);

            var targetProvince = await _provinceService.GetById(dataModel.ProvinceCode);

            if (targetProvince.Data == null)
                return GenericResult<CandidateDTO>.Error((int)HttpStatusCode.NotFound,
                    "Province is not found.");

            targetCandidate.Province = targetProvince.Data.Name;

            _candidateRepository.Update(targetCandidate);
            await _candidateRepository.SaveAsync();

            var response = _mapper.Map<CandidateDTO>(targetCandidate);
            response.ProvinceCode = dataModel.ProvinceCode;

            return GenericResult<CandidateDTO>.Success(response);
        }

        public async Task<GenericResult<CandidateDTO>> UpdateStatus(Guid id, WorkingStatusEnum status)
        {
            var targetCandidate = await _candidateRepository.GetById(id);

            if (targetCandidate == null)
                return GenericResult<CandidateDTO>.Error((int)HttpStatusCode.NotFound,
                    "Candidate is not found.");

            targetCandidate.Status = status;
            _candidateRepository.Update(targetCandidate);
            await _candidateRepository.SaveAsync();

            var response = _mapper.Map<CandidateDTO>(targetCandidate);

            return GenericResult<CandidateDTO>.Success(response);
        }
    }
}
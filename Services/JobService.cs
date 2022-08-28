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
using VoiceAPI.Models.Data.JobInvitations;
using VoiceAPI.Models.Data.Jobs;
using VoiceAPI.Models.Data.Notifications;
using VoiceAPI.Models.Data.TransactionHistories;
using VoiceAPI.Models.Payload.Jobs;
using VoiceAPI.Models.Responses.JobInvitations;
using VoiceAPI.Models.Responses.Jobs;
using VoiceAPI.Models.Responses.Wallets;

namespace VoiceAPI.Services
{
    public class JobService : IJobService
    {
        private readonly IMapper _mapper;

        private readonly IJobRepository _jobRepository;

        private readonly ISubCategoryService _subCategoryService;
        private readonly ITransactionHistoryService _transactionHistoryService;
        private readonly IJobInvitationService _jobInvitationService;
        private readonly ICandidateService _candidateService;
        private readonly IEnterpriseService _enterpriseService;
        private readonly INotificationService _notificationService;

        public JobService(IMapper mapper,
            IJobRepository jobRepository,
            ISubCategoryService subCategoryService,
            ITransactionHistoryService transactionHistoryService,
            IJobInvitationService jobInvitationService,
            ICandidateService candidateService,
            IEnterpriseService enterpriseService,
            INotificationService notificationService)
        {
            _mapper = mapper;

            _jobRepository = jobRepository;

            _subCategoryService = subCategoryService;
            _transactionHistoryService = transactionHistoryService;
            _jobInvitationService = jobInvitationService;
            _candidateService = candidateService;
            _enterpriseService = enterpriseService;
            _notificationService = notificationService;
        }

        public async Task<GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>>
            EnterpriseInviteCandidateForWorking(JobInvitationJobCreateDataModel dataModel)
        {
            var targetEnterprise = await _enterpriseService.GetById(dataModel.JobDataModel.EnterpriseId);

            if (targetEnterprise.Data == null)
                return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Error((int)HttpStatusCode.NotFound,
                    "Enterprise is not found.");

            var enterpriseAvailableBalance =
                await _jobRepository.GetAvailableBalanceOfEnterprise(dataModel.JobDataModel.EnterpriseId);

            if (enterpriseAvailableBalance == -1)
                return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_21",
                    "Ví của nhà tuyển dụng này chưa được khởi tạo.");

            if (enterpriseAvailableBalance < dataModel.JobDataModel.Price)
                return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_28",
                    "AvailableBalance không đủ để mời ứng viên vô làm việc.");

            var targetJob = _mapper.Map<Job>(dataModel.JobDataModel);

            if ((targetJob.MinuteDuration == null || targetJob.MinuteDuration == 0)
                && (targetJob.DayDuration == null || targetJob.DayDuration == 0)
                && (targetJob.HourDuration == null || targetJob.HourDuration == 0))
                return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_24",
                    "Tất cả thời lượng không thể bằng 0.");

            if (targetJob.MinuteDuration == 0)
                targetJob.MinuteDuration = null;

            if (targetJob.DayDuration == 0)
                targetJob.DayDuration = null;

            if (targetJob.HourDuration == 0)
                targetJob.HourDuration = null;

            if (dataModel.JobDataModel.Price <= decimal.Zero)
                return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_58",
                    "Giá phải là một số dương.");

            var subCategory = await _subCategoryService.GetById(dataModel.JobDataModel.SubCategoryId);

            if (!subCategory.IsSuccess)
                return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Error((int)HttpStatusCode.NotFound,
                    "SubCategory is not found.");

            var targetCandidate = await _candidateService.GetById(dataModel.CandidateId);

            if (targetCandidate.Data == null)
                return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Error((int)HttpStatusCode.NotFound,
                    "Candidate is not found.");

            if (targetCandidate.Data.Status != WorkingStatusEnum.AVAILABLE)
                return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_61",
                    "Candidate is currently unavailable.");

            targetJob.IsPublic = false;

            _jobRepository.Create(targetJob);
            await _jobRepository.SaveAsync();

            // Action for JobInvitation

            var jobInvitationDataModel = new JobInvitationCreateDataModel
            {
                Id = targetJob.Id,
                CandidateId = dataModel.CandidateId
            };

            var targetJobInvitation = await _jobInvitationService.CreateNew(jobInvitationDataModel);

            // Action for Wallet
            var targetTransactionDataModel = new TransactionHistoryEnterpriseInviteCandidateForWorkingDataModel
            {
                WalletId = dataModel.JobDataModel.EnterpriseId,
                JobId = targetJob.Id,
                Amount = dataModel.JobDataModel.Price
            };

            var response = await _transactionHistoryService.InviteCandidateForWorking(targetTransactionDataModel);

            response.Data.Transaction.Job.JobInvitation = _mapper.Map<JobInvitationDTO>(targetJobInvitation.Data);

            await _notificationService.PostNotifyCandidateInvited(new NotifyCandidateInvitedDataModel
            {
                EnterpriseId = dataModel.JobDataModel.EnterpriseId,
                TargetAccountId = dataModel.CandidateId,
                JobId = targetJob.Id
            });

            return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Success(response.Data);
        }

        public async Task<GenericResult<WalletEnterprisePostJobDTO>> EnterprisePostJob(
            JobEnterprisePostJobDataModel dataModel)
        {
            var enterpriseAvailableBalance =
                await _jobRepository.GetAvailableBalanceOfEnterprise(dataModel.EnterpriseId);

            if (enterpriseAvailableBalance == -1)
                return GenericResult<WalletEnterprisePostJobDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_21",
                    "Ví của nhà tuyển dụng chưa được khởi tạo.");

            if (enterpriseAvailableBalance < dataModel.Price)
                return GenericResult<WalletEnterprisePostJobDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_19",
                    "AvailableBalance không đủ để đăng bài đăng này.");

            var targetJob = _mapper.Map<Job>(dataModel);
            targetJob.IsPublic = true;

            if ((targetJob.MinuteDuration == null || targetJob.MinuteDuration == 0)
                && (targetJob.DayDuration == null || targetJob.DayDuration == 0)
                && (targetJob.HourDuration == null || targetJob.HourDuration == 0))
                return GenericResult<WalletEnterprisePostJobDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_24",
                    "Tất cả thời lượng không thể bằng 0.");

            if (targetJob.MinuteDuration == 0)
                targetJob.MinuteDuration = null;

            if (targetJob.DayDuration == 0)
                targetJob.DayDuration = null;

            if (targetJob.HourDuration == 0)
                targetJob.HourDuration = null;

            if (dataModel.Price <= decimal.Zero)
                return GenericResult<WalletEnterprisePostJobDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_58",
                    "Giá phải là một số dương.");

            var subCategory = await _subCategoryService.GetById(dataModel.SubCategoryId);

            if (!subCategory.IsSuccess)
                return GenericResult<WalletEnterprisePostJobDTO>.Error((int)HttpStatusCode.NotFound,
                    "SubCategory is not found.");

            targetJob.CreatedTime = DateTime.UtcNow;
            _jobRepository.Create(targetJob);
            await _jobRepository.SaveAsync();

            // Action for TransactionHistory & Wallet

            var targetTransactionHistoryDataModel = new TransactionHistoryEnterprisePostJobDataModel
            {
                WalletId = dataModel.EnterpriseId,
                JobId = targetJob.Id,
                Amount = dataModel.Price
            };

            var response = await _transactionHistoryService.EnterprisePostJob(targetTransactionHistoryDataModel);

            return GenericResult<WalletEnterprisePostJobDTO>.Success(response.Data);
        }

        public async Task<GenericResult<JobDTO>> GetById(Guid id)
        {
            var targetJob = await _jobRepository.GetById(id);

            if (targetJob == null)
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.NotFound,
                    "Job is not found.");

            var response = _mapper.Map<JobDTO>(targetJob);

            return GenericResult<JobDTO>.Success(response);
        }

        public async Task<GenericResult<List<JobDTO>>> GetEnterpriseOwnJob(Guid enterpriseId)
        {
            var targetEnterprise = await _enterpriseService.GetById(enterpriseId);

            if (targetEnterprise == null)
                return GenericResult<List<JobDTO>>.Error((int)HttpStatusCode.NotFound,
                    "Enterprise is not found.");

            var targetJobs = await _jobRepository.Get().AsNoTracking()
                .Where(tempJob => tempJob.EnterpriseId.CompareTo(enterpriseId) == 0)
                .ToListAsync();

            var response = _mapper.Map<List<JobDTO>>(targetJobs);

            return GenericResult<List<JobDTO>>.Success(response);
        }

        public async Task<GenericResult<List<JobDTO>>> FilterSearch(JobSearchFilterPayload payload)
        {
            var targetJobs = await _jobRepository
                .Get()
                .Where(tempJob => tempJob.IsPublic
                                  && tempJob.JobStatus != JobStatusEnum.DELETED
                                  && tempJob.JobStatus != JobStatusEnum.FINISHED)
                .ToListAsync();

            if (payload.CategoryId != null)
            {
                var targetSubCategories =
                    await _subCategoryService.GetAllByCategoryId(payload.CategoryId ?? Guid.Empty);

                for (var i = 0; i < targetJobs.Count; i++)
                {
                    if (!targetSubCategories.Data
                            .AsEnumerable()
                            .Select(tempSub => tempSub.Id)
                            .ToList().Contains(targetJobs[i].SubCategoryId))
                    {
                        targetJobs.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(payload.searchValue))
            {
                for (var i = 0; i < targetJobs.Count; i++)
                {
                    if (!targetJobs[i].Name
                            .Trim()
                            .ToLower()
                            .Contains(payload.searchValue.ToLower()))
                    {
                        targetJobs.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (payload.MinPrice != null)
            {
                for (var i = 0; i < targetJobs.Count; i++)
                {
                    if (decimal.Compare(targetJobs[i].Price, payload.MinPrice ?? decimal.Zero) < 0)
                    {
                        targetJobs.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (payload.MaxPrice != null)
            {
                for (var i = 0; i < targetJobs.Count; i++)
                {
                    if (decimal.Compare(targetJobs[i].Price, payload.MaxPrice ?? decimal.Zero) > 0)
                    {
                        targetJobs.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (payload.Tone != null)
            {
                for (var i = 0; i < targetJobs.Count; i++)
                {
                    if (targetJobs[i].Tone != payload.Tone)
                    {
                        targetJobs.RemoveAt(i);
                        i--;
                    }
                }
            }

            var response = _mapper.Map<List<JobDTO>>(targetJobs);

            return GenericResult<List<JobDTO>>.Success(response);
        }

        public async Task<GenericResult<JobDTO>> DeleteJob(JobDeleteDataModel dataModel)
        {
            var targetJob = await _jobRepository.GetById(dataModel.Id);

            if (targetJob == null)
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.NotFound,
                    "Job is not found.");

            if (targetJob.EnterpriseId.CompareTo(dataModel.EnterpriseId) != 0)
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_15",
                    "Công việc này không thuộc về doanh nghiệp này.");

            if (targetJob.JobStatus == JobStatusEnum.DELETED)
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_53",
                    "Công việc này đã bị xóa.");

            if (targetJob.JobStatus != JobStatusEnum.PENDING)
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_54",
                    "Không thể xóa công việc đang chờ xử lý.");

            targetJob.JobStatus = JobStatusEnum.DELETED;

            _jobRepository.Update(targetJob);
            await _jobRepository.SaveAsync();

            // Action for Transaction History & Wallet
            var targetTransactionHistory = await _transactionHistoryService.DeleteJob(targetJob);

            var response = _mapper.Map<JobDTO>(targetJob);

            return GenericResult<JobDTO>.Success(response);
        }

        public async Task<GenericResult<JobDTO>> UpdateJob(JobUpdateDataModel dataModel)
        {
            var targetJob = await _jobRepository.GetById(dataModel.Id);

            if (targetJob == null)
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.NotFound,
                    "Job is not found.");

            if (targetJob.EnterpriseId.CompareTo(dataModel.EnterpriseId) != 0)
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_15",
                    "Công việc này không thuộc về doanh nghiệp này.");

            if (targetJob.JobStatus != JobStatusEnum.PENDING)
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_52",
                    "Không thể cập nhật công việc đang chờ xử lý.");

            if (dataModel.MinuteDuration != null
                && dataModel.HourDuration != null
                && dataModel.DayDuration != null
                && dataModel.MinuteDuration == 0
                && dataModel.HourDuration == 0
                && dataModel.DayDuration == 0)
            {
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_59",
                    "Tất cả thời lượng không thể bằng 0.");
            }

            if (dataModel.Price != null)
            {
                var jobOrders = await _jobRepository.GetNotRejectedOrders(targetJob.Id);

                if (jobOrders.Count > 0)
                    return GenericResult<JobDTO>.Error((int)HttpStatusCode.BadRequest,
                        "V400_57",
                        "Không thể cập nhật giá cho công việc chưa từ chối đơn hàng.");

                if (dataModel.Price <= decimal.Zero)
                    return GenericResult<JobDTO>.Error((int)HttpStatusCode.BadRequest,
                        "V400_58",
                        "Giá phải là một số dương.");

                // Action for Transaction History & Wallet
                await _transactionHistoryService.UpdateJob(targetJob, dataModel.Price ?? decimal.Zero);
            }

            _mapper.Map(dataModel, targetJob);

            _jobRepository.Update(targetJob);
            await _jobRepository.SaveAsync();

            var response = _mapper.Map<JobDTO>(targetJob);

            return GenericResult<JobDTO>.Success(response);
        }

        public async Task<GenericResult<JobDTO>> UpdateStatusAfterJobApproved(Guid id)
        {
            var targetJob = await _jobRepository.GetById(id);

            if (targetJob == null)
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.NotFound,
                    "Job is not found.");

            targetJob.JobStatus = Entities.Enums.JobStatusEnum.PROCESSING;

            _jobRepository.Update(targetJob);
            await _jobRepository.SaveAsync();

            var response = _mapper.Map<JobDTO>(targetJob);

            return GenericResult<JobDTO>.Success(response);
        }

        public async Task<GenericResult<JobDTO>> UpdateStatusAfterJobFinished(Guid id)
        {
            var targetJob = await _jobRepository.GetById(id);

            if (targetJob == null)
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.NotFound,
                    "Job is not found.");

            targetJob.JobStatus = Entities.Enums.JobStatusEnum.FINISHED;

            _jobRepository.Update(targetJob);
            await _jobRepository.SaveAsync();

            var response = _mapper.Map<JobDTO>(targetJob);

            return GenericResult<JobDTO>.Success(response);
        }
    }
}
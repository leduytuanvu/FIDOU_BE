using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;
using VoiceAPI.IRepositories;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Reports;
using VoiceAPI.Models.Data.TransactionHistories;
using VoiceAPI.Models.Payload.Reports;
using VoiceAPI.Models.Responses.Reports;

namespace VoiceAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly IMapper _mapper;

        private readonly IReportRepository _reportRepository;
        private readonly ICandidateRepository _candidateRepository;

        private readonly IOrderService _orderService;
        private readonly ITransactionHistoryService _transactionHistoryService;

        public ReportService(IMapper mapper,
            IReportRepository reportRepository,
            IOrderService orderService,
            ITransactionHistoryService transactionHistoryService,
            ICandidateRepository candidateRepository)
        {
            _mapper = mapper;

            _reportRepository = reportRepository;
            _candidateRepository = candidateRepository;

            _orderService = orderService;
            _transactionHistoryService = transactionHistoryService;
        }

        public async Task<GenericResult<ReportDTO>> AdminReview(ReportAdminReviewPayload payload)
        {
            var targetReport = await _reportRepository.GetById(payload.Id);

            if (targetReport == null)
                return GenericResult<ReportDTO>.Error((int)HttpStatusCode.NotFound,
                    "Report is not found.");

            if (targetReport.IsReviewed)
                return GenericResult<ReportDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_47",
                    "Báo cáo này đã được xem xét.");

            _mapper.Map(payload, targetReport);

            targetReport.IsReviewed = true;
            targetReport.UpdatedTime = DateTime.UtcNow;

            _reportRepository.Update(targetReport);
            await _reportRepository.SaveAsync();

            var targetCandidate = await _reportRepository.GetCandidateByReportId(payload.Id);

            var targetJob = await _reportRepository.GetJobByReportId(payload.Id);

            if (payload.IsTrue)
            {
                // Action for Order
                await _orderService.UpdateStatus(payload.Id, OrderStatusEnum.FINISHED);

                // Action for Job
                await _reportRepository.UpdateJobStatus(targetJob.Id, JobStatusEnum.FINISHED);

                // Action for Candidate
                targetCandidate.Status = WorkingStatusEnum.AVAILABLE;
                await _candidateRepository.UpdateCandidateStatusAfterReview(targetCandidate);
                
                // Action for TransactionHistory & Wallet
                var transactionDataModel = new TransactionHistoryReportReviewTrueDataModel
                {
                    CandidateId = targetCandidate.Id,
                    EnterpriseId = targetJob.EnterpriseId,
                    JobId = targetJob.Id
                };

                await _transactionHistoryService.ActionForTrueReport(transactionDataModel);
            }

            var response = _mapper.Map<ReportDTO>(targetReport);

            return GenericResult<ReportDTO>.Success(response);
        }

        public async Task<GenericResult<List<ReportDTO>>> GetAll()
        {
            var targetReports = await _reportRepository
                .Get()
                .OrderBy(tempReport => tempReport.CreatedTime)
                .ToListAsync();

            List<ReportDTO> responses = new();
            foreach (var tempReport in targetReports)
            {
                var response = _mapper.Map<ReportDTO>(tempReport);

                var targetCandidate = await _reportRepository.GetCandidateByReportId(tempReport.Id);
                response.CandidateId = targetCandidate.Id;
                response.CandidateEmail = targetCandidate.EmailContact;
                responses.Add(response);

                var targetEnterprise = await _reportRepository.GetEnterpriseByReportId(tempReport.Id);
                response.EnterpriseEmail = targetEnterprise.EmailContact;
            }

            return GenericResult<List<ReportDTO>>.Success(responses);
        }

        public async Task<GenericResult<ReportDTO>> CreateNew(ReportCreateDataModel dataModel)
        {
            var targetOrder = await _orderService.GetById(dataModel.Payload.Id);

            if (targetOrder.Data == null)
                return GenericResult<ReportDTO>.Error((int)HttpStatusCode.NotFound,
                    "Order is not found.");

            if (targetOrder.Data.Status != Entities.Enums.OrderStatusEnum.PROCESSING)
                return GenericResult<ReportDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_41",
                    "Order's status pahir đang diễn ra mới được báo cáo.");

            var targetReport = await _reportRepository.GetById(dataModel.Payload.Id);

            if (targetReport != null)
                return GenericResult<ReportDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_39",
                    "Đã báo cáo.");

            if (!(await _orderService.IsOrderBelongToEnterprise(dataModel.Payload.Id, dataModel.EnterpriseId)).Data)
                return GenericResult<ReportDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_40",
                    "Chỉ có thể báo cáo công việc của bạn.");

            targetReport = _mapper.Map<Report>(dataModel.Payload);
            targetReport.CreatedTime = DateTime.UtcNow;
            targetReport.IsReviewed = false;


            _reportRepository.Create(targetReport);
            await _reportRepository.SaveAsync();

            var response = _mapper.Map<ReportDTO>(targetReport);

            return GenericResult<ReportDTO>.Success(response);
        }
    }
}
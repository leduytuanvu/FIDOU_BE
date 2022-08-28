﻿using AutoMapper;
using System;
using System.Net;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;
using VoiceAPI.IRepositories;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Notifications;
using VoiceAPI.Models.Data.Orders;
using VoiceAPI.Models.Data.TransactionHistories;
using VoiceAPI.Models.Responses.Jobs;
using VoiceAPI.Models.Responses.Orders;
using VoiceAPI.Models.Responses.Wallets;

namespace VoiceAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;

        private readonly IOrderRepository _orderRepository;

        private readonly ITransactionHistoryService _transactionHistoryService;
        private readonly IJobService _jobService;
        private readonly ICandidateService _candidateService;
        private readonly INotificationService _notificationService;

        public OrderService(IMapper mapper,
            IOrderRepository orderRepository,
            ITransactionHistoryService transactionHistoryService,
            IJobService jobService,
            ICandidateService candidateService,
            INotificationService notificationService)
        {
            _mapper = mapper;

            _orderRepository = orderRepository;

            _transactionHistoryService = transactionHistoryService;
            _jobService = jobService;
            _candidateService = candidateService;
            _notificationService = notificationService;
        }

        public async Task<GenericResult<WalletCandidateApplyJobDTO>> CandidateApplyForJob(
            OrderCandidateApplyForJobDataModel dataModel)
        {
            var targetOrder = await _orderRepository
                .GetByCandidateIdAndJobId(dataModel.CandidateId, dataModel.Payload.JobId);

            if (targetOrder != null)
                return GenericResult<WalletCandidateApplyJobDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_14",
                    "Bạn đã nộp đơn cho công việc này.");

            var candidateAvailableBalance =
                await _orderRepository.GetAvailableBalanceOfCandidate(dataModel.CandidateId);

            if (candidateAvailableBalance == -1)
                return GenericResult<WalletCandidateApplyJobDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_22",
                    "Ví của nhà tuyển dụng chưa được khởi tạo.");

            var targetJob = await _jobService.GetById(dataModel.Payload.JobId);

            if (candidateAvailableBalance < decimal.Multiply(targetJob.Data.Price, 0.1m)) // 10% of JobPrice
                return GenericResult<WalletCandidateApplyJobDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_23",
                    "AvailableBalance không đủ để nộp đơn vô công việc này.");

            if (targetJob.Data.JobStatus != Entities.Enums.JobStatusEnum.PENDING)
                return GenericResult<WalletCandidateApplyJobDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_26",
                    "Chỉ có thể nộp đơn cho công việc đang chờ xử lý.");

            var targetCandidate = await _candidateService.GetById(dataModel.CandidateId);

            if (targetCandidate.Data.Status != Entities.Enums.WorkingStatusEnum.AVAILABLE)
                return GenericResult<WalletCandidateApplyJobDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_25",
                    "Bạn chưa hoàn thành công việc hiện tại.");

            targetOrder = new Order
            {
                CandidateId = dataModel.CandidateId,
                CreatedTime = DateTime.UtcNow,
                Status = Entities.Enums.OrderStatusEnum.PENDING,
                JobId = dataModel.Payload.JobId
            };

            _orderRepository.Create(targetOrder);
            await _orderRepository.SaveAsync();

            // Action for TransactionHistory & Wallet

            var targetTransactionHistoryDataModel = new TransactionHistoryCandidateApplyJobDataModel
            {
                Amount = targetJob.Data.Price,
                JobId = dataModel.Payload.JobId,
                WalletId = dataModel.CandidateId
            };

            var response = await _transactionHistoryService.CandidateApplyJob(targetTransactionHistoryDataModel);

            var targetEnterprise = await _orderRepository.GetEnterpriseByOrderId(targetOrder.Id);

            await _notificationService.PostNotifyJobHaveNewApplicant(new NotifyJobHaveNewApplicantDataModel
            {
                CandidateId = dataModel.CandidateId,
                JobId = dataModel.Payload.JobId,
                TargetAccountId = targetEnterprise.Id
            });

            return GenericResult<WalletCandidateApplyJobDTO>.Success(response.Data);
        }

        public async Task<GenericResult<OrderEnterpriseApproveJobDTO>> EnterpriseApproveJob(
            OrderEnterpriseApproveJobDataModel dataModel)
        {
            var targetOrder =
                await _orderRepository.GetByIdAndJobId(dataModel.Payload.OrderId, dataModel.Payload.JobId);

            if (targetOrder == null)
                return GenericResult<OrderEnterpriseApproveJobDTO>.Error((int)HttpStatusCode.NotFound,
                    "Order is not found.");

            if (targetOrder.Status != Entities.Enums.OrderStatusEnum.PENDING)
                return GenericResult<OrderEnterpriseApproveJobDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_16",
                    "Chỉ có thể nộp đơn cho công việc đang chờ xử lý.");

            targetOrder.Status = Entities.Enums.OrderStatusEnum.PROCESSING;
            targetOrder.UpdatedTime = DateTime.UtcNow;

            var enterprise = await _orderRepository
                .CheckIfJobIsBelongToEnterprise(dataModel.EnterpriseId, dataModel.Payload.JobId);

            if (enterprise == null)
                return GenericResult<OrderEnterpriseApproveJobDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_15",
                    "Công việc này không thuộc nhà tuyển dụng này.");

            var orders = await _orderRepository.GetAllWithJobId(dataModel.Payload.JobId);

            foreach (var order in orders)
            {
                if (order.Id.CompareTo(dataModel.Payload.OrderId) != 0)
                {
                    order.Status = Entities.Enums.OrderStatusEnum.REJECTED;
                    order.UpdatedTime = DateTime.UtcNow;

                    _orderRepository.Update(order);
                }
            }

            // Update status of Candidate
            await _candidateService.UpdateStatus(targetOrder.CandidateId, Entities.Enums.WorkingStatusEnum.UNAVAILABLE);

            // Action for TransactionHistory & Wallet

            var approveTransactionHistoryDataModel = new TransactionHistoryEnterpriseApproveJobDataModel
            {
                OrderId = dataModel.Payload.OrderId,
                AppliedOrders = orders
            };

            await _transactionHistoryService.EnterpriseApproveJob(approveTransactionHistoryDataModel);

            // Update status
            await _jobService.UpdateStatusAfterJobApproved(dataModel.Payload.JobId);
            await UpdateStatusAfterJobApproved(targetOrder.Id);

            _orderRepository.Update(targetOrder);
            await _orderRepository.SaveAsync();

            var targetJob = await _jobService.GetById(dataModel.Payload.JobId);

            var response = _mapper.Map<OrderEnterpriseApproveJobDTO>(targetOrder);
            response.Job = _mapper.Map<JobDTO>(targetJob.Data);

            return GenericResult<OrderEnterpriseApproveJobDTO>.Success(response);
        }

        public async Task<GenericResult<WalletOrderFinishDTO>> FinishOrder(Guid id)
        {
            var targetOrder = await _orderRepository.GetById(id);

            if (targetOrder == null)
                return GenericResult<WalletOrderFinishDTO>.Error((int)HttpStatusCode.NotFound,
                    "Order is not found.");

            if (targetOrder.Status != OrderStatusEnum.PROCESSING)
                return GenericResult<WalletOrderFinishDTO>.Error((int)HttpStatusCode.BadRequest,
                    "V400_54",
                    "Chỉ có thể kết thúc công việc đang diễn ra.");

            targetOrder.Status = Entities.Enums.OrderStatusEnum.FINISHED;
            targetOrder.UpdatedTime = DateTime.UtcNow;

            _orderRepository.Update(targetOrder);
            await _orderRepository.SaveAsync();

            await _jobService.UpdateStatusAfterJobFinished(targetOrder.JobId);

            // Update status for Candidate
            await _candidateService.UpdateStatus(targetOrder.CandidateId, Entities.Enums.WorkingStatusEnum.AVAILABLE);

            // Action for TransactionHistory & Wallet
            var response = await _transactionHistoryService.FinishOrder(targetOrder.Id);

            return GenericResult<WalletOrderFinishDTO>.Success(response.Data);
        }

        public async Task<GenericResult<OrderDTO>> GetById(Guid id)
        {
            var order = await _orderRepository.GetById(id);

            if (order == null)
                return GenericResult<OrderDTO>.Error((int)HttpStatusCode.NotFound,
                    "Order is not found.");

            var response = _mapper.Map<OrderDTO>(order);

            return GenericResult<OrderDTO>.Success(response);
        }

        public async Task<GenericResult<bool>> IsOrderBelongToEnterprise(Guid id, Guid enterpriseId)
        {
            var response = await _orderRepository.IsOrderBelongToEnterprise(id, enterpriseId);

            return GenericResult<bool>.Success(response);
        }

        public async Task<GenericResult<OrderDTO>> UpdateStatus(Guid id, OrderStatusEnum status)
        {
            var targetOrder = await _orderRepository.GetById(id);

            if (targetOrder == null)
                return GenericResult<OrderDTO>.Error((int)HttpStatusCode.NotFound,
                    "Order is not found.");

            targetOrder.Status = status;
            targetOrder.UpdatedTime = DateTime.UtcNow;

            _orderRepository.Update(targetOrder);
            await _orderRepository.SaveAsync();

            var response = _mapper.Map<OrderDTO>(targetOrder);

            return GenericResult<OrderDTO>.Success(response);
        }

        public async Task<GenericResult<OrderDTO>> UpdateStatusAfterJobApproved(Guid id)
        {
            var targetOrder = await _orderRepository.GetById(id);

            if (targetOrder == null)
                return GenericResult<OrderDTO>.Error((int)HttpStatusCode.NotFound,
                    "Order is not found.");

            targetOrder.Status = Entities.Enums.OrderStatusEnum.PROCESSING;
            targetOrder.UpdatedTime = DateTime.UtcNow;

            _orderRepository.Update(targetOrder);
            await _orderRepository.SaveAsync();

            var response = _mapper.Map<OrderDTO>(targetOrder);

            return GenericResult<OrderDTO>.Success(response);
        }
    }
}
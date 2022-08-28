using AutoMapper;
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
using VoiceAPI.Models.Data.TransactionHistories;
using VoiceAPI.Models.Responses.JobInvitations;
using VoiceAPI.Models.Responses.Jobs;
using VoiceAPI.Models.Responses.Orders;
using VoiceAPI.Models.Responses.TransactionHistories;
using VoiceAPI.Models.Responses.Wallets;

namespace VoiceAPI.Services
{
    public class TransactionHistoryService : ITransactionHistoryService
    {
        private readonly IMapper _mapper;

        private readonly ITransactionHistoryRepository _transactionHistoryRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IJobRepository _jobRepository;

        private readonly IAdminService _adminService;

        public TransactionHistoryService(IMapper mapper, 
            ITransactionHistoryRepository transactionHistoryRepository, 
            IWalletRepository walletRepository, 
            IAdminService adminService, 
            IJobRepository jobRepository)
        {
            _mapper = mapper;

            _transactionHistoryRepository = transactionHistoryRepository;
            _walletRepository = walletRepository;

            _adminService = adminService;
            _jobRepository = jobRepository;
        }

        public async Task<GenericResult<WalletAdminDepositBalanceDTO>> AdminDepositBalance(TransactionHistoryAdminCreateDataModel dataModel)
        {
            // Validate Wallet data
            var targetWallet = await _walletRepository.GetWalletByDepositCode(dataModel.Payload.DepositCode);

            if (targetWallet == null)
                return GenericResult<WalletAdminDepositBalanceDTO>.Error((int)HttpStatusCode.NotFound,
                                        "Wallet is not found.");

            // Validate TransactionHistory data
            if (dataModel.Payload.Amount <= 0)
                return GenericResult<WalletAdminDepositBalanceDTO>.Error((int)HttpStatusCode.Forbidden,
                                            "V400_18",
                                            "Số tiền phải lớn hơn 0.");

            // Action for TransactionHistory 
            var targetTransactionHistory = _mapper.Map<TransactionHistory>(dataModel.Payload);

            targetTransactionHistory.AdminId = dataModel.AdminId;
            targetTransactionHistory.TransactionType = Entities.Enums.TransactionTypeEnum.DEPOSIT;
            targetTransactionHistory.CreatedTime = DateTime.UtcNow;
            targetTransactionHistory.WalletId = targetWallet.Id;

            _transactionHistoryRepository.Create(targetTransactionHistory);
            await _transactionHistoryRepository.SaveAsync();

            // Action for Wallet
            targetWallet = await _transactionHistoryRepository
                                        .UpdateBalanceAfterDeposit(targetWallet.Id, dataModel.Payload.Amount);


            var response = _mapper.Map<WalletAdminDepositBalanceDTO>(targetWallet);
            response.Transaction = _mapper.Map<TransactionHistoryAdminDepositBalanceDTO>(targetTransactionHistory);

            return GenericResult<WalletAdminDepositBalanceDTO>.Success(response);
        }

        public async Task<GenericResult<WalletEnterprisePostJobDTO>> EnterprisePostJob(TransactionHistoryEnterprisePostJobDataModel dataModel)
        {
            // Validate Wallet date
            var targetWallet = await _walletRepository.GetById(dataModel.WalletId);

            if (targetWallet == null)
                return GenericResult<WalletEnterprisePostJobDTO>.Error((int)HttpStatusCode.NotFound,
                                        "Wallet is not found.");

            // Validate TransactionHistory data
            if (dataModel.Amount <= 0)
                return GenericResult<WalletEnterprisePostJobDTO>.Error((int)HttpStatusCode.Forbidden,
                                            "V400_20",
                                            "Số tiền phải lớn hơn 0.");
            // Action for TransactionHistory
            var targetTransactionHistory = _mapper.Map<TransactionHistory>(dataModel);
            targetTransactionHistory.TransactionType = Entities.Enums.TransactionTypeEnum.SEND;
            targetTransactionHistory.CreatedTime = DateTime.UtcNow;

            _transactionHistoryRepository.Create(targetTransactionHistory);
            await _transactionHistoryRepository.SaveAsync();

            // Action for Wallet
            targetWallet = await _transactionHistoryRepository
                                        .UpdateBalanceAfterPostJob(dataModel.WalletId, dataModel.Amount);


            var response = _mapper.Map<WalletEnterprisePostJobDTO>(targetWallet);
            response.Transaction = _mapper.Map<TransactionHistoryEnterprisePostJobDTO>(targetTransactionHistory);

            var targetJob = await _transactionHistoryRepository.GetJobByJobId(dataModel.JobId);
            response.Transaction.Job = _mapper.Map<JobDTO>(targetJob);

            return GenericResult<WalletEnterprisePostJobDTO>.Success(response);
        }

        public async Task<GenericResult<WalletCandidateApplyJobDTO>> CandidateApplyJob(TransactionHistoryCandidateApplyJobDataModel dataModel)
        {
            // Validate Wallet date
            var targetWallet = await _walletRepository.GetById(dataModel.WalletId);

            if (targetWallet == null)
                return GenericResult<WalletCandidateApplyJobDTO>.Error((int)HttpStatusCode.NotFound,
                                        "Wallet is not found.");

            // Validate TransactionHistory data
            if (dataModel.Amount <= 0)
                return GenericResult<WalletCandidateApplyJobDTO>.Error((int)HttpStatusCode.Forbidden,
                                            "V400_20",
                                            "Số tiền phải lớn hơn 0.");

            // Action for TransactionHistory
            var targetTransactionHistory = _mapper.Map<TransactionHistory>(dataModel);
            targetTransactionHistory.TransactionType = Entities.Enums.TransactionTypeEnum.SEND;
            targetTransactionHistory.CreatedTime = DateTime.UtcNow;
            targetTransactionHistory.Amount = decimal.Multiply(dataModel.Amount, 0.1m);

            _transactionHistoryRepository.Create(targetTransactionHistory);
            await _transactionHistoryRepository.SaveAsync();

            // Action for Wallet
            targetWallet = await _transactionHistoryRepository
                                        .UpdateBalanceAfterApplyJob(dataModel.WalletId, dataModel.Amount);


            var response = _mapper.Map<WalletCandidateApplyJobDTO>(targetWallet);
            response.Transaction = _mapper.Map<TransactionHistoryCandidateApplyJobDTO>(targetTransactionHistory);

            var targetOrder = await _transactionHistoryRepository.GetOrderByCandidateIdAndJobId(dataModel.WalletId, dataModel.JobId);
            response.Transaction.Order = _mapper.Map<OrderCandidateApplyJobDTO>(targetOrder);

            var targetJob = await _transactionHistoryRepository.GetJobByJobId(dataModel.JobId);
            response.Transaction.Order.Job = _mapper.Map<JobDTO>(targetJob);

            return GenericResult<WalletCandidateApplyJobDTO>.Success(response);
        }

        public async Task<GenericResult<JobDTO>> EnterpriseApproveJob(TransactionHistoryEnterpriseApproveJobDataModel dataModel)
        {
            var targetOrder = await _transactionHistoryRepository.GetOrderByOrderId(dataModel.OrderId);

            if (targetOrder == null)
                return GenericResult<JobDTO>.Error((int)HttpStatusCode.NotFound,
                                    "Order is not found.");

            var choosenCandidate =
                await _transactionHistoryRepository.GetCandidateByOrderId(dataModel.OrderId);

            var targetJob = await _transactionHistoryRepository.GetJobByOrderId(dataModel.OrderId);

            foreach (var appliedOrder in dataModel.AppliedOrders)
            {
                if (appliedOrder.CandidateId.CompareTo(choosenCandidate.Id) == 0)
                    continue;

                var targetTransactionHistory = new TransactionHistory
                {
                    TransactionType = Entities.Enums.TransactionTypeEnum.REFUNDED, 
                    Amount = decimal.Multiply(targetJob.Price, 0.1m), // 10% of JobPrice
                    JobId = targetJob.Id, 
                    CreatedTime = DateTime.UtcNow, 
                    WalletId = appliedOrder.CandidateId, 
                };

                await _walletRepository.UpdateBalanceAfterRejected(appliedOrder.CandidateId, targetJob.Price);

                _transactionHistoryRepository.Create(targetTransactionHistory);
                await _transactionHistoryRepository.SaveAsync();
            }

            var response = _mapper.Map<JobDTO>(targetJob);

            return GenericResult<JobDTO>.Success(response);
        }

        public async Task<GenericResult<WalletOrderFinishDTO>> FinishOrder(Guid orderId)
        {
            var targetOrder = await _transactionHistoryRepository.GetOrderByOrderId(orderId);
            var targetJob = await _transactionHistoryRepository.GetJobByOrderId(orderId);
            var targetEnterprise = await _transactionHistoryRepository.GetEnterpriseByOrderId(orderId);
            var targetCandidate = await _transactionHistoryRepository.GetCandidateByOrderId(orderId);

            // Action for Wallets
            var targetEnterpriseWallet = await _walletRepository.UpdateEnterpriseBalanceAfterFinishOrder(targetEnterprise.Id, targetJob.Price);
            await _walletRepository.UpdateCandidateBalanceAfterFinishOrder(targetCandidate.Id, targetJob.Price);
            await _walletRepository.SaveAsync();

            // Action for TransactionHistories

            var enterpriseTransactionHistory = new TransactionHistory
            {
                JobId = targetJob.Id,
                CreatedTime = DateTime.UtcNow,
                Amount = targetJob.Price,
                TransactionType = Entities.Enums.TransactionTypeEnum.UNLOCK,
                WalletId = targetEnterprise.Id
            };

            _transactionHistoryRepository.Create(enterpriseTransactionHistory);

            var candidateTransactionHistory = new TransactionHistory
            {
                JobId = targetJob.Id,
                CreatedTime = DateTime.UtcNow,
                Amount = decimal.Multiply(targetJob.Price, 0.9m), // Receive 90% of JobPrice - 10% is pay for system
                TransactionType = Entities.Enums.TransactionTypeEnum.RECEIVE,
                WalletId = targetCandidate.Id
            };

            _transactionHistoryRepository.Create(candidateTransactionHistory);

            await _transactionHistoryRepository.SaveAsync();

            var response = _mapper.Map<WalletOrderFinishDTO>(targetEnterpriseWallet); // Enterprise trigger this method,
                                                                                      // so response is return to Enterprise
            response.Order = _mapper.Map<OrderFinishDTO>(targetOrder);
            response.Order.Transaction = 
                _mapper.Map<TransactionHistoryOrderFinishDTO>(enterpriseTransactionHistory);
            response.Order.Transaction.Job = 
                _mapper.Map<JobDTO>(targetJob);

            return GenericResult<WalletOrderFinishDTO>.Success(response);
        }

        public async Task<GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>> InviteCandidateForWorking(TransactionHistoryEnterpriseInviteCandidateForWorkingDataModel dataModel)
        {
            // Validate Wallet date
            var targetWallet = await _walletRepository.GetById(dataModel.WalletId);

            if (targetWallet == null)
                return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Error((int)HttpStatusCode.NotFound,
                                        "Wallet is not found.");

            // Validate TransactionHistory data
            if (dataModel.Amount < 0)
                return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Error((int)HttpStatusCode.Forbidden,
                                            "V400_20",
                                            "Số tiền phải lớn hơn 0.");
            // Action for TransactionHistory
            var targetTransactionHistory = _mapper.Map<TransactionHistory>(dataModel);
            targetTransactionHistory.TransactionType = Entities.Enums.TransactionTypeEnum.SEND;
            targetTransactionHistory.CreatedTime = DateTime.UtcNow;

            _transactionHistoryRepository.Create(targetTransactionHistory);
            await _transactionHistoryRepository.SaveAsync();

            // Action for Wallet
            targetWallet = await _walletRepository
                            .UpdateBalanceAfterInviteCandidateForWorking(dataModel.WalletId, dataModel.Amount);

            var response = 
                _mapper.Map<WalletEnterpriseInviteCandidateForWorkingDTO>(targetWallet);
            
            response.Transaction = 
                _mapper.Map<TransactionHistoryEnterpriseInviteCandidateForWorkingDTO>(targetTransactionHistory);

            var targetJob = await _transactionHistoryRepository.GetJobByJobId(dataModel.JobId);
            response.Transaction.Job = _mapper.Map<JobWithInvitationDTO>(targetJob);

            response.Transaction.Job.JobInvitation = new JobInvitationDTO();

            return GenericResult<WalletEnterpriseInviteCandidateForWorkingDTO>.Success(response);
        }

        public async Task<GenericResult<WalletCandidateReplyInvitationDTO>> AcceptJobInvitation(TransactionHistoryCandidateAcceptInvitationDataModel dataModel)
        {
            // Validate TransactionHistory data
            if (dataModel.Amount <= 0)
                return GenericResult<WalletCandidateReplyInvitationDTO>.Error((int)HttpStatusCode.Forbidden,
                                            "V400_20",
                                            "Số tiền phải lớn hơn 0.");

            // Action for TransactionHistory
            var targetTransactionHistory = new TransactionHistory
            {
                Amount = decimal.Multiply(dataModel.Amount, 0.1m),
                WalletId = dataModel.WalletId,
                CreatedTime = DateTime.UtcNow,
                JobId = dataModel.JobId,
                TransactionType = Entities.Enums.TransactionTypeEnum.SEND
            };

            _transactionHistoryRepository.Create(targetTransactionHistory);
            await _transactionHistoryRepository.SaveAsync();

            // Action for Wallet
            var targetWallet = await _walletRepository.UpdateBalanceAfterAcceptInvitation(dataModel.WalletId, dataModel.Amount);

            var response = _mapper.Map<WalletCandidateReplyInvitationDTO>(targetWallet);

            response.JobInvitation = new JobInvitationWithJobDetailDTO();

            response.Transaction = _mapper.Map<TransactionHistoryCandidateReplyInvitationDTO>(targetTransactionHistory);
            response.Transaction.Order = new OrderCandidateReplyInvitationDTO();

            return GenericResult<WalletCandidateReplyInvitationDTO>.Success(response);
        }
        
        public async Task<GenericResult<WalletCandidateReplyInvitationDTO>> RejectJobInvitation(TransactionHistoryCandidateRejectInvitationDataModel dataModel)
        {
            // Validate TransactionHistory data
            if (dataModel.Amount <= 0)
                return GenericResult<WalletCandidateReplyInvitationDTO>.Error((int)HttpStatusCode.Forbidden,
                    "V400_20",
                    "Số tiền phải lớn hơn 0.");

            // Action for TransactionHistory
            var targetTransactionHistory = new TransactionHistory
            {
                Amount = dataModel.Amount,
                WalletId = dataModel.WalletId,
                CreatedTime = DateTime.UtcNow,
                JobId = dataModel.JobId,
                TransactionType = TransactionTypeEnum.REFUNDED
            };

            _transactionHistoryRepository.Create(targetTransactionHistory);
            await _transactionHistoryRepository.SaveAsync();

            // Action for Wallet
            var targetWallet = await _walletRepository.UpdateBalanceAfterRejectInvitation(dataModel.WalletId, dataModel.Amount);

            var response = _mapper.Map<WalletCandidateReplyInvitationDTO>(targetWallet);

            response.JobInvitation = new JobInvitationWithJobDetailDTO();

            response.Transaction = _mapper.Map<TransactionHistoryCandidateReplyInvitationDTO>(targetTransactionHistory);
            response.Transaction.Order = new OrderCandidateReplyInvitationDTO();

            return GenericResult<WalletCandidateReplyInvitationDTO>.Success(response);
        }

        public async Task<GenericResult<TransactionHistoryForBothRole>> ActionForTrueReport(TransactionHistoryReportReviewTrueDataModel dataModel)
        {
            var targetJob = await _transactionHistoryRepository.GetJobByJobId(dataModel.JobId);

            // Action for Wallet & TransactionHistory of Enterprise
            await _walletRepository.UpdateEnterpriseBalanceAfterReviewTrue(dataModel.EnterpriseId, targetJob.Price);

            var enterpriseTransactionHistory = new TransactionHistory
            {
                Amount = targetJob.Price, 
                JobId = dataModel.JobId, 
                CreatedTime = DateTime.UtcNow, 
                WalletId = dataModel.EnterpriseId, 
                TransactionType = Entities.Enums.TransactionTypeEnum.REFUNDED
            };

            _transactionHistoryRepository.Create(enterpriseTransactionHistory);

            // Action for Wallet & TransactionHistory of Candidate
            await _walletRepository.UpdateCandidateBalanceAfterReviewTrue(dataModel.CandidateId, targetJob.Price);

            var candidateTransactionHistory = new TransactionHistory
            {
                Amount = decimal.Multiply(targetJob.Price, 0.1m),
                JobId = dataModel.JobId,
                CreatedTime = DateTime.UtcNow,
                WalletId = dataModel.CandidateId,
                TransactionType = Entities.Enums.TransactionTypeEnum.SEND
            };

            _transactionHistoryRepository.Create(candidateTransactionHistory);

            await _transactionHistoryRepository.SaveAsync();

            var response = new TransactionHistoryForBothRole
            {
                EnterpriseTransactionHistory = _mapper.Map<TransactionHistoryDTO>(enterpriseTransactionHistory),
                CandidateTransactionHistory = _mapper.Map<TransactionHistoryDTO>(candidateTransactionHistory)
            };

            return GenericResult<TransactionHistoryForBothRole>.Success(response);
        }

        public async Task<GenericResult<List<TransactionHistoryGetDTO>>> GetOwnTransactionHistories(Guid id)
        {
            var targetTransactionHistories = await _transactionHistoryRepository.GetTransactionHistoriesByAccountId(id);

            var response = _mapper.Map<List<TransactionHistoryGetDTO>>(targetTransactionHistories);

            for (var i = 0; i < response.Count; i++)
            {
                response[i].AdminEmail =
                    targetTransactionHistories[i].AdminId != null
                        ? (await _adminService.GetById(targetTransactionHistories[i].AdminId ?? Guid.Empty)).Data.Email
                        : null;
                
                response[i].JobName =
                    targetTransactionHistories[i].JobId != null
                        ? (await _jobRepository.GetById(targetTransactionHistories[i].JobId ?? Guid.Empty)).Name
                        : null;
            }

            return GenericResult<List<TransactionHistoryGetDTO>>.Success(response);
        }

        public async Task<GenericResult<WalletWithTransactionsDTO>> UpdateJob(Job targetJob, decimal newPrice)
        {
            var targetWallet = await _walletRepository.GetById(targetJob.EnterpriseId);
            
            var transactionHistory = new TransactionHistory
            {
                JobId = targetJob.Id, 
                CreatedTime = DateTime.UtcNow, 
                WalletId = targetJob.EnterpriseId
            };

            if (targetJob.Price != newPrice)
            {
                if (targetJob.Price < newPrice)
                {
                    var gapAmount = decimal.Subtract(newPrice, targetJob.Price);
                    if (gapAmount > targetWallet.AvailableBalance) 
                        return GenericResult<WalletWithTransactionsDTO>.Error((int)HttpStatusCode.Forbidden,
                            "V400_56",
                            "Số tiền trong bạn có không đủ để cập nhật công việc này.");
                    
                    transactionHistory.Amount = decimal.Subtract(newPrice, targetJob.Price);
                    transactionHistory.TransactionType = TransactionTypeEnum.SEND;
                }
                else
                {
                    transactionHistory.Amount = decimal.Subtract(targetJob.Price, newPrice);
                    transactionHistory.TransactionType = TransactionTypeEnum.REFUNDED;
                }
            
                // Action for Wallet
                targetWallet = await _walletRepository.UpdateBalanceAfterUpdateJob(targetJob.EnterpriseId, 
                    transactionHistory.Amount, 
                    transactionHistory.TransactionType == TransactionTypeEnum.REFUNDED);
            
                _transactionHistoryRepository.Create(transactionHistory);
                await _transactionHistoryRepository.SaveAsync();
            }
            
            var response = _mapper.Map<WalletWithTransactionsDTO>(targetWallet);
            response.TransactionHistories =
                _mapper.Map<List<TransactionHistoryDTO>>(await _transactionHistoryRepository.GetTransactionHistoriesByAccountId(targetJob.EnterpriseId));

            return GenericResult<WalletWithTransactionsDTO>.Success(response);
        }

        public async Task<GenericResult<WalletWithTransactionsDTO>> DeleteJob(Job targetJob)
        {
            var enterpriseTransactionHistory = new TransactionHistory
            {
                JobId = targetJob.Id, 
                CreatedTime = DateTime.UtcNow, 
                WalletId = targetJob.EnterpriseId, 
                TransactionType = TransactionTypeEnum.REFUNDED, 
                Amount = targetJob.Price
            };
            
            // Action for Wallet
            var targetWallet =
                await _walletRepository.UpdateEnterpriseBalanceAfterDeleteJob(targetJob.EnterpriseId, targetJob.Price);

            _transactionHistoryRepository.Create(enterpriseTransactionHistory);

            var targetCandidates = await _jobRepository.GetCandidatesAppliedForJob(targetJob.Id);

            var targetCandidatesId = targetCandidates
                .AsEnumerable()
                .Select(temp => temp.Id)
                .ToList();
            var candidatesWalletIds =
                await _walletRepository.UpdateCandidatesBalanceAfterDeleteJob(targetCandidatesId, targetJob.Price);

            foreach (var candidateId in candidatesWalletIds)
            {
                var candidateTransactionHistory = new TransactionHistory
                {
                    JobId = targetJob.Id, 
                    CreatedTime = DateTime.UtcNow, 
                    WalletId = candidateId, 
                    TransactionType = TransactionTypeEnum.REFUNDED, 
                    Amount = decimal.Multiply(targetJob.Price, 0.1m)
                };
                
                _transactionHistoryRepository.Create(candidateTransactionHistory);
            }
            
            await _transactionHistoryRepository.SaveAsync();
            
            var response = _mapper.Map<WalletWithTransactionsDTO>(targetWallet);
            response.TransactionHistories =
                _mapper.Map<List<TransactionHistoryDTO>>(await _transactionHistoryRepository.GetTransactionHistoriesByAccountId(targetJob.EnterpriseId));
            
            return GenericResult<WalletWithTransactionsDTO>.Success(response);
        }
    }
}

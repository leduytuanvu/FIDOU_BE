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
using VoiceAPI.Models.Data.ConversationSchedules;
using VoiceAPI.Models.Data.Notifications;
using VoiceAPI.Models.Payload.ConversationSchedules;
using VoiceAPI.Models.Responses.ConversationSchedules;

namespace VoiceAPI.Services
{
    public class ConversationScheduleService : IConversationScheduleService
    {
        private readonly IMapper _mapper;

        private readonly IConversationScheduleRepository _conversationScheduleRepository;

        private readonly ICandidateService _candidateService;
        private readonly IEnterpriseService _enterpriseService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;

        public ConversationScheduleService(IMapper mapper,
            IConversationScheduleRepository conversationScheduleRepository, 
            ICandidateService candidateService, 
            IEnterpriseService enterpriseService, 
            INotificationService notificationService, 
            IOrderService orderService)
        {
            _mapper = mapper;

            _conversationScheduleRepository = conversationScheduleRepository;

            _candidateService = candidateService;
            _enterpriseService = enterpriseService;
            _notificationService = notificationService;
            _orderService = orderService;
        }

        public async Task<GenericResult<ConversationScheduleDTO>> EnterpriseCreateConversationSchedule(ConversationScheduleCreateDataModel dataModel)
        {
            var targetCandidate = await _candidateService.GetById(dataModel.CandidateId);

            if (targetCandidate.Data == null)
                return GenericResult<ConversationScheduleDTO>.Error((int)HttpStatusCode.NotFound,
                                            "Candidate is not found.");
            
            var targetOrder = await _orderService.GetById(dataModel.OrderId);
            if (targetOrder.Data == null) 
                return GenericResult<ConversationScheduleDTO>.Error((int)HttpStatusCode.NotFound, 
                    "Order is not found.");

            if (DateTime.Compare(dataModel.ScheduledTime, DateTime.UtcNow) <= 0) 
                return GenericResult<ConversationScheduleDTO>.Error((int)HttpStatusCode.BadRequest,
                                            "V400_43",
                                            "Lịch hẹn phải sớm hơn thời gian hiện tại.");

            var targetConversationSchedule = _mapper.Map<ConversationSchedule>(dataModel);

            _conversationScheduleRepository.Create(targetConversationSchedule);
            await _conversationScheduleRepository.SaveAsync();

            var notifyDataModel = new NotifyConversationScheduleDataModel
            {
                EnterpriseId = dataModel.EnterpriseId,
                TargetAccountId = dataModel.CandidateId,
                ScheduledTime = dataModel.ScheduledTime
            };

            await _notificationService.PostNotifyConversationScheduled(notifyDataModel);
            
            var response = _mapper.Map<ConversationScheduleDTO>(targetConversationSchedule);

            return GenericResult<ConversationScheduleDTO>.Success(response);
        }

        public async Task<GenericResult<ConversationScheduleDTO>> GetByEnterpriseIdAndCandidateId(Guid enterpriseId, Guid candidateId)
        {
            var targetCandidate = await _candidateService.GetById(candidateId);
            if (targetCandidate.Data == null)
                return GenericResult<ConversationScheduleDTO>.Error((int)HttpStatusCode.NotFound,
                                            "Candidate is not found.");

            var targetEnterprise = await _enterpriseService.GetById(enterpriseId);
            if (targetEnterprise.Data == null)
                return GenericResult<ConversationScheduleDTO>.Error((int)HttpStatusCode.NotFound,
                                            "Enterprise is not found.");

            var targetConversationSchedule = await _conversationScheduleRepository
                                                    .GetByEnterpriseIdAndCandidateId(enterpriseId, candidateId);

            if (targetConversationSchedule == null)
                return GenericResult<ConversationScheduleDTO>.Error((int)HttpStatusCode.NotFound, 
                                            "ConversationSchedule is not found.");

            var response = _mapper.Map<ConversationScheduleDTO>(targetConversationSchedule);

            return GenericResult<ConversationScheduleDTO>.Success(response);
        }

        public async Task<GenericResult<ConversationScheduleDTO>> GetByOrderId(Guid orderId)
        {
            var targetOrder = await _orderService.GetById(orderId);
            if (targetOrder.Data == null) 
                return GenericResult<ConversationScheduleDTO>.Error((int)HttpStatusCode.NotFound, 
                    "Order is not found.");
            
            var targetConversationSchedule = await _conversationScheduleRepository
                .GetByOrderId(orderId);

            if (targetConversationSchedule == null)
                return GenericResult<ConversationScheduleDTO>.Error((int)HttpStatusCode.NotFound, 
                    "ConversationSchedule is not found.");

            var response = _mapper.Map<ConversationScheduleDTO>(targetConversationSchedule);

            return GenericResult<ConversationScheduleDTO>.Success(response);
        }
    }
}

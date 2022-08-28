using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Notifications;
using VoiceAPI.Models.Payload.Notifications;
using VoiceAPI.Models.Responses.Notifications;

namespace VoiceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/notifications")]
    [ApiController]
    [ApiVersion("2")]
    public class NotificationController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly INotificationService _notificationService;

        public NotificationController(IMapper mapper, 
            INotificationService notificationService)
        {
            _mapper = mapper;

            _notificationService = notificationService;
        }

        [HttpPost]
        [MapToApiVersion("2")]
        public async Task<IActionResult> PostNotification(NotificationPayload payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _notificationService.PostNotification(payload);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<NotifyBaseDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
        
        [HttpPost("candidate-invited")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> NotifyCandidateInvited(NotifyCandidateInvitedDataModel dataModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _notificationService.PostNotifyCandidateInvited(dataModel);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<NotifyCandidateInvitedDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
        
        [HttpPost("job-have-new-applicant")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> NotifyJobHaveNewApplicant(NotifyJobHaveNewApplicantDataModel dataModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _notificationService.PostNotifyJobHaveNewApplicant(dataModel);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<NotifyJobHaveNewApplicantDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
        
        [HttpPost("conversation-schedule")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> NotifyConversationSchedule(NotifyConversationScheduleDataModel dataModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _notificationService.PostNotifyConversationScheduled(dataModel);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<NotifyConversationScheduleDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
    }
}

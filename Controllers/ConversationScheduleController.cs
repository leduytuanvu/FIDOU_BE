using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VoiceAPI.Helpers;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.ConversationSchedules;
using VoiceAPI.Models.Payload.ConversationSchedules;
using VoiceAPI.Models.Responses.ConversationSchedules;

namespace VoiceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/conversation-schedules")]
    [ApiController]
    [ApiVersion("1")]
    public class ConversationScheduleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITools _tools;

        private readonly IConversationScheduleService _conversationScheduleService;

        public ConversationScheduleController(IMapper mapper, ITools tools,
            IConversationScheduleService conversationScheduleService)
        {
            _mapper = mapper;
            _tools = tools;

            _conversationScheduleService = conversationScheduleService;
        }

        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ENTERPRISE")]
        public async Task<IActionResult> CreateNew(ConversationScheduleCreatePayload payload)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataModel = _mapper.Map<ConversationScheduleCreateDataModel>(payload);
            dataModel.EnterpriseId = Guid.Parse(id);

            var result = await _conversationScheduleService.EnterpriseCreateConversationSchedule(dataModel);

            // Return with statusCode=201 and data if success
            if (result.IsSuccess)
                return StatusCode((int)HttpStatusCode.Created, 
                    new SingleObjectResponse<ConversationScheduleDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetByEnterpriseIdAndCandidateId(Guid enterpriseId, Guid candidateId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _conversationScheduleService.GetByEnterpriseIdAndCandidateId(enterpriseId, candidateId);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<ConversationScheduleDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
        
        [HttpGet("order/{orderId}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetByOrderId(Guid orderId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _conversationScheduleService.GetByOrderId(orderId);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<ConversationScheduleDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
    }
}

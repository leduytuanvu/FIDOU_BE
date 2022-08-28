using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceAPI.Helpers;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Responses.TransactionHistories;

namespace VoiceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/transaction-histories")]
    [ApiController]
    [ApiVersion("1")]
    public class TransactionHistoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITools _tools;

        private readonly ITransactionHistoryService _transactionHistoryService;

        public TransactionHistoryController(IMapper mapper, ITools tools,
            ITransactionHistoryService transactionHistoryService)
        {
            _mapper = mapper;
            _tools = tools;

            _transactionHistoryService = transactionHistoryService;
        }
        
        [HttpGet]
        [MapToApiVersion("1")]
        [Authorize(Roles = "CANDIDATE,ENTERPRISE")]
        public async Task<IActionResult> GetOwnTransactionHistory()
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _transactionHistoryService.GetOwnTransactionHistories(Guid.Parse(id));

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new MultiObjectResponse<TransactionHistoryGetDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
    }
}
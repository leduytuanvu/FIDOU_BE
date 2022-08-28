using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Helpers;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Responses.Wallets;

namespace VoiceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/wallets")]
    [ApiController]
    [ApiVersion("1")]
    public class WalletController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITools _tools;

        private readonly IWalletService _walletService;

        public WalletController(IMapper mapper, ITools tools,
            IWalletService walletService)
        {
            _mapper = mapper;
            _tools = tools;

            _walletService = walletService;
        }
        
        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _walletService.GetById(id);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<WalletWithTransactionsDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
    }
}

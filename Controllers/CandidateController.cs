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
using VoiceAPI.Models.Data.Candidates;
using VoiceAPI.Models.Data.Orders;
using VoiceAPI.Models.Payload.Candidates;
using VoiceAPI.Models.Payload.Orders;
using VoiceAPI.Models.Responses.Candidates;
using VoiceAPI.Models.Responses.Orders;

namespace VoiceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/candidates")]
    [ApiController]
    [ApiVersion("1")]
    public class CandidateController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITools _tools;

        private readonly ICandidateService _candidateService;

        public CandidateController(IMapper mapper, ITools tools, 
            ICandidateService candidateService)
        {
            _mapper = mapper;
            _tools = tools;

            _candidateService = candidateService;
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _candidateService.GetProfile(id);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<CandidateGetProfileDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
        
        [HttpGet("review-points")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetSortingByReviewPoint([FromQuery]CandidateGetWithSortingReviewPointPayload payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _candidateService.GetWithSortingReviewPoint(payload);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new MultiObjectResponse<CandidateGetProfileDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> SearchFilter([FromQuery]CandidateSearchFilterPayload payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _candidateService.SearchFilter(payload);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new MultiObjectResponse<CandidateGetProfileDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize(Roles = "CANDIDATE")]
        public async Task<IActionResult> CreateNew(CandidateCreatePayload payload)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataModel = _mapper.Map<CandidateCreateDataModel>(payload);
            dataModel.Id = Guid.Parse(id);

            var result = await _candidateService.CreateNew(dataModel);

            // Return with statusCode=201 and data if success
            if (result.IsSuccess)
                return StatusCode((int)HttpStatusCode.Created, new SingleObjectResponse<CandidateDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpPut]
        [MapToApiVersion("1")]
        [Authorize(Roles = "CANDIDATE")]
        public async Task<IActionResult> Update(CandidateUpdatePayload payload)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataModel = _mapper.Map<CandidateUpdateDataModel>(payload);
            dataModel.Id = Guid.Parse(id);

            var result = await _candidateService.UpdateProfile(dataModel);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<CandidateDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpGet("order-histories")]
        [MapToApiVersion("1")]
        [Authorize(Roles = "CANDIDATE")]
        public async Task<IActionResult> GetOrderHistory([FromQuery]OrderCandidateGetHistoryPayload payload)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataModel = new OrderCandidateGetHistoryDataModel
            {
                Payload = payload, 
                CandidateId = Guid.Parse(id)
            };

            var result = await _candidateService.GetOrderHistory(dataModel);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new MultiObjectResponse<OrderHistoryDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
    }
}

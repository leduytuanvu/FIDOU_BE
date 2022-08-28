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
using VoiceAPI.Models.Data.Reports;
using VoiceAPI.Models.Payload.Reports;
using VoiceAPI.Models.Responses.Reports;

namespace VoiceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/reports")]
    [ApiController]
    [ApiVersion("1")]
    public class ReportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITools _tools;

        private readonly IReportService _reportService;

        public ReportController(IMapper mapper, ITools tools, 
            IReportService reportService)
        {
            _mapper = mapper;
            _tools = tools;

            _reportService = reportService;
        }

        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ENTERPRISE")]
        public async Task<IActionResult> CreateReport(ReportCreatePayload payload)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataModel = new ReportCreateDataModel
            {
                EnterpriseId = Guid.Parse(id),
                Payload = payload
            };

            var result = await _reportService.CreateNew(dataModel);

            // Return with statusCode=201 and data if success
            if (result.IsSuccess)
                return StatusCode((int)HttpStatusCode.Created, 
                    new SingleObjectResponse<ReportDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpPut("review")]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AdminReview(ReportAdminReviewPayload payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _reportService.AdminReview(payload);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<ReportDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
        
        [HttpGet]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _reportService.GetAll();

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new MultiObjectResponse<ReportDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
    }
}

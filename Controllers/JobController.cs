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
using VoiceAPI.Models.Data.JobInvitations;
using VoiceAPI.Models.Data.Jobs;
using VoiceAPI.Models.Payload.JobInvitations;
using VoiceAPI.Models.Payload.Jobs;
using VoiceAPI.Models.Responses.Jobs;
using VoiceAPI.Models.Responses.Wallets;

namespace VoiceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/jobs")]
    [ApiController]
    [ApiVersion("1")]
    public class JobController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITools _tools;

        private readonly IJobService _jobService;

        public JobController(IMapper mapper, ITools tools, 
            IJobService jobService)
        {
            _mapper = mapper;
            _tools = tools;

            _jobService = jobService;
        }
        
        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _jobService.GetById(id);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<JobDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ENTERPRISE")]
        public async Task<IActionResult> CreateJob(JobEnterprisePostJobPayload payload)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataModel = _mapper.Map<JobEnterprisePostJobDataModel>(payload);
            dataModel.EnterpriseId = Guid.Parse(id);

            var result = await _jobService.EnterprisePostJob(dataModel);

            // Return with statusCode=201 and data if success
            if (result.IsSuccess)
                return StatusCode((int)HttpStatusCode.Created, 
                            new SingleObjectResponse<WalletEnterprisePostJobDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
        
        [HttpDelete("{jobId}")]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ENTERPRISE")]
        public async Task<IActionResult> DeleteJob(Guid jobId)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataModel = new JobDeleteDataModel
            {
                Id = jobId,
                EnterpriseId = Guid.Parse(id)
            };

            var result = await _jobService.DeleteJob(dataModel);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<JobDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
        
        [HttpPut]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ENTERPRISE")]
        public async Task<IActionResult> UpdateJob(JobUpdatePayload payload)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataModel = _mapper.Map<JobUpdateDataModel>(payload);
            dataModel.EnterpriseId = Guid.Parse(id);

            var result = await _jobService.UpdateJob(dataModel);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<JobDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpGet]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ENTERPRISE")]
        public async Task<IActionResult> GetOwnJobs()
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _jobService.GetEnterpriseOwnJob(Guid.Parse(id));

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new MultiObjectResponse<JobDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpPost("invite-candidate")]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ENTERPRISE")]
        public async Task<IActionResult> InviteCandidateForWorking(JobInvitationJobCreatePayload payload)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jobDataModel = _mapper.Map<JobEnterpriseInviteCandidateForWorkingDataModel>(payload.JobPayload);
            jobDataModel.EnterpriseId = Guid.Parse(id);

            var dataModel = new JobInvitationJobCreateDataModel
            {
                CandidateId = payload.CandidateId, 
                JobDataModel = jobDataModel
            };

            var result = await _jobService.EnterpriseInviteCandidateForWorking(dataModel);

            // Return with statusCode=201 and data if success
            if (result.IsSuccess)
                return StatusCode((int)HttpStatusCode.Created,
                            new SingleObjectResponse<WalletEnterpriseInviteCandidateForWorkingDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
        
        [HttpGet("search")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> SearchFilter([FromQuery]JobSearchFilterPayload payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _jobService.FilterSearch(payload);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new MultiObjectResponse<JobDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
    }
}

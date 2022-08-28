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
using VoiceAPI.Models.Data.VoiceDemos;
using VoiceAPI.Models.Payload.VoiceDemos;
using VoiceAPI.Models.Responses.VoiceDemos;

namespace VoiceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/voice-demos")]
    [ApiController]
    [ApiVersion("1")]
    public class VoiceDemoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITools _tools;

        private readonly IVoiceDemoService _voiceDemoService;

        public VoiceDemoController(IMapper mapper, ITools tools,
            IVoiceDemoService voiceDemoService)
        {
            _mapper = mapper;
            _tools = tools;

            _voiceDemoService = voiceDemoService;
        }

        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize(Roles = "CANDIDATE")]
        public async Task<IActionResult> CreateNew(VoiceDemoCreatePayload payload)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataModel = _mapper.Map<VoiceDemoCreateDataModel>(payload);
            dataModel.CandidateId = Guid.Parse(id);

            var result = await _voiceDemoService.CreateNew(dataModel);

            // Return with statusCode=201 and data if success
            if (result.IsSuccess)
                return StatusCode((int)HttpStatusCode.Created, 
                    new SingleObjectResponse<VoiceDemoDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
        
        [HttpPut("text-transcript")]
        [MapToApiVersion("1")]
        [Authorize(Roles = "CANDIDATE")]
        public async Task<IActionResult> UpdateTextTranscript(VoiceDemoUpdateTextTranscriptPayload payload)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataModel = _mapper.Map<VoiceDemoUpdateTextTranscriptDataModel>(payload);
            dataModel.CandidateId = Guid.Parse(id);

            var result = await _voiceDemoService.UpdateTextTranscript(dataModel);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<VoiceDemoDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetAll()
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _voiceDemoService.GetAllByCandidateId(Guid.Parse(id));

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new MultiObjectResponse<VoiceDemoDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
    }
}

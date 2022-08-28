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
using VoiceAPI.Models.Payload.Wards;
using VoiceAPI.Models.Responses.Wards;

namespace VoiceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/wards")]
    [ApiController]
    [ApiVersion("1")]
    public class WardController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITools _tools;

        private readonly IWardService _wardService;

        public WardController(IMapper mapper, ITools tools, 
            IWardService wardService)
        {
            _mapper = mapper;
            _tools = tools;

            _wardService = wardService;
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _wardService.GetAll();

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new MultiObjectResponse<WardDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _wardService.GetById(id);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<WardDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateAllNew(List<WardCreatePayload> payloads)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _wardService.CreateAllNew(payloads);

            // Return with statusCode=201 and data if success
            if (result.IsSuccess)
                return StatusCode((int)HttpStatusCode.Created, 
                    new MultiObjectResponse<WardDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
    }
}

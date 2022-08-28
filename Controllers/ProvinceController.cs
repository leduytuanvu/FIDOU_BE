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
using VoiceAPI.Models.Payload.Provinces;
using VoiceAPI.Models.Responses.Provinces;

namespace VoiceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/provinces")]
    [ApiController]
    [ApiVersion("1")]
    public class ProvinceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITools _tools;

        private readonly IProvinceService _provinceService;

        public ProvinceController(IMapper mapper, ITools tools, 
            IProvinceService provinceService)
        {
            _mapper = mapper;
            _tools = tools;

            _provinceService = provinceService;
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _provinceService.GetAll();

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new MultiObjectResponse<ProvinceDTO>(result.Data));

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

            var result = await _provinceService.GetById(id);

            // Return with statusCode=200 and data if success
            if (result.IsSuccess)
                return Ok(new SingleObjectResponse<ProvinceDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }

        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetById(List<ProvinceCreatePayload> payloads)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _provinceService.CreateAllNew(payloads);

            // Return with statusCode=201 and data if success
            if (result.IsSuccess)
                return StatusCode((int)HttpStatusCode.Created, 
                    new MultiObjectResponse<ProvinceDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
    }
}

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
using VoiceAPI.Models.Data.Reviews;
using VoiceAPI.Models.Payload.Reviews;
using VoiceAPI.Models.Responses.Reviews;

namespace VoiceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/reviews")]
    [ApiController]
    [ApiVersion("1")]
    public class ReviewController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITools _tools;

        private readonly IReviewService _reviewService;

        public ReviewController(IMapper mapper, ITools tools, 
            IReviewService reviewService)
        {
            _mapper = mapper;
            _tools = tools;

            _reviewService = reviewService;
        }

        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize(Roles = "ENTERPRISE")]
        public async Task<IActionResult> EnterpriseCreateReview(ReviewEnterpriseCreatePayload payload)
        {
            var id = _tools.GetUserOfRequest(User.Claims);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataModel = new ReviewEnterpriseCreateDataModel
            {
                EnterpriseId = Guid.Parse(id),
                Payload = payload
            };

            var result = await _reviewService.EnterpriseCreateReview(dataModel);

            // Return with statusCode=201 and data if success
            if (result.IsSuccess)
                return StatusCode((int)HttpStatusCode.Created, 
                    new SingleObjectResponse<ReviewDTO>(result.Data));

            // Add error response data informations
            Response.StatusCode = result.StatusCode;

            var response = _mapper.Map<ErrorResponse>(result);

            return StatusCode(result.StatusCode, response);
        }
    }
}

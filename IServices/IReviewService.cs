using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Reviews;
using VoiceAPI.Models.Payload.Reviews;
using VoiceAPI.Models.Responses.Reviews;

namespace VoiceAPI.IServices
{
    public interface IReviewService
    {
        Task<GenericResult<ReviewDTO>> EnterpriseCreateReview(ReviewEnterpriseCreateDataModel dataModel);
    }
}

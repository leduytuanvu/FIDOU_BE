using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities.Enums;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Candidates;
using VoiceAPI.Models.Data.Orders;
using VoiceAPI.Models.Payload.Candidates;
using VoiceAPI.Models.Responses.Candidates;
using VoiceAPI.Models.Responses.Orders;

namespace VoiceAPI.IServices
{
    public interface ICandidateService
    {
        Task<GenericResult<CandidateDTO>> GetById(Guid id);
        Task<GenericResult<CandidateGetProfileDTO>> GetProfile(Guid id);
        Task<GenericResult<CandidateDTO>> UpdateProfile(CandidateUpdateDataModel dataModel);
        Task<GenericResult<CandidateDTO>> CreateNew(CandidateCreateDataModel dataModel);
        Task<GenericResult<CandidateDTO>> UpdateStatus(Guid id, WorkingStatusEnum status);
        Task<GenericResult<List<CandidateGetProfileDTO>>> SearchFilter(CandidateSearchFilterPayload payload);
        Task<GenericResult<List<OrderHistoryDTO>>> GetOrderHistory(OrderCandidateGetHistoryDataModel dataModel);

        Task<GenericResult<List<CandidateGetProfileDTO>>> GetWithSortingReviewPoint(
            CandidateGetWithSortingReviewPointPayload payload);
    }
}

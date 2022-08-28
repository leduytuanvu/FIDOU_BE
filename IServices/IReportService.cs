using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Reports;
using VoiceAPI.Models.Payload.Reports;
using VoiceAPI.Models.Responses.Reports;

namespace VoiceAPI.IServices
{
    public interface IReportService
    {
        Task<GenericResult<ReportDTO>> CreateNew(ReportCreateDataModel dataModel);
        Task<GenericResult<ReportDTO>> AdminReview(ReportAdminReviewPayload payload);
        Task<GenericResult<List<ReportDTO>>> GetAll();
    }
}

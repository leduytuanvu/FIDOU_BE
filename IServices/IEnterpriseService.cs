using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Enterprises;
using VoiceAPI.Models.Data.Orders;
using VoiceAPI.Models.Payload.Enterprises;
using VoiceAPI.Models.Responses.Enterprises;
using VoiceAPI.Models.Responses.Jobs;
using VoiceAPI.Models.Responses.Orders;

namespace VoiceAPI.IServices
{
    public interface IEnterpriseService
    {
        Task<GenericResult<EnterpriseWithJobsDTO>> GetById(Guid id);
        Task<GenericResult<EnterpriseDTO>> UpdateProfile(EnterpriseUpdateDataModel dataModel);
        Task<GenericResult<EnterpriseDTO>> CreateNew(EnterpriseCreateDataModel dataModel);
        Task<GenericResult<List<JobWithOrdersDTO>>> GetOrderHistory(OrderEnterpriseGetHistoryDataModel dataModel);
    }
}

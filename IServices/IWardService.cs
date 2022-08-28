using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Payload.Wards;
using VoiceAPI.Models.Responses.Wards;

namespace VoiceAPI.IServices
{
    public interface IWardService
    {
        Task<GenericResult<List<WardDTO>>> CreateAllNew(List<WardCreatePayload> payloads);
        Task<GenericResult<List<WardDTO>>> GetAll();
        Task<GenericResult<WardDTO>> GetById(string code);
        Task<GenericResult<WardDTO>> GetByName(string name);
    }
}

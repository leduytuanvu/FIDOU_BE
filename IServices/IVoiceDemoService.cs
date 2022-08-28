using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.VoiceDemos;
using VoiceAPI.Models.Responses.VoiceDemos;

namespace VoiceAPI.IServices
{
    public interface IVoiceDemoService
    {
        Task<GenericResult<VoiceDemoDTO>> CreateNew(VoiceDemoCreateDataModel dataModel);
        Task<GenericResult<List<VoiceDemoDTO>>> GetAllByCandidateId(Guid candidateId);
        Task<GenericResult<VoiceDemoDTO>> UpdateTextTranscript(VoiceDemoUpdateTextTranscriptDataModel dataModel);
    }
}

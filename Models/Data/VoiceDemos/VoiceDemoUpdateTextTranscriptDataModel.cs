using System;

namespace VoiceAPI.Models.Data.VoiceDemos
{
    public class VoiceDemoUpdateTextTranscriptDataModel
    {
        public Guid Id { get; set; }
        
        public Guid CandidateId { get; set; }
        
        public string TextTranscript { get; set; }
    }
}
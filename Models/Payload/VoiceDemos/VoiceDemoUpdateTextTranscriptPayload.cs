using System;

namespace VoiceAPI.Models.Payload.VoiceDemos
{
    public class VoiceDemoUpdateTextTranscriptPayload
    {
        public Guid Id { get; set; }
        
        public string TextTranscript { get; set; }
    }
}
using System;
using VoiceAPI.Entities.Enums;

namespace VoiceAPI.Models.Payload.Jobs
{
    public class JobSearchFilterPayload
    {
        public string searchValue { get; set; }
        
        public Guid? CategoryId { get; set; }
        
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        
        public ToneEnum? Tone { get; set; }
    }
}
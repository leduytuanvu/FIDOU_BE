using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities.Enums;

namespace VoiceAPI.Models.Data.VoiceDemos
{
    public class VoiceDemoCreateDataModel
    {
        public Guid CandidateId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public Guid SubCategoryId { get; set; }
        
        public ToneEnum Tone { get; set; }
        
        public string TextTranscript { get; set; }
    }
}

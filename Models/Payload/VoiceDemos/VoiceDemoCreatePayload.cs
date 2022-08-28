using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities.Enums;

namespace VoiceAPI.Models.Payload.VoiceDemos
{
    public class VoiceDemoCreatePayload
    {
        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public string Url { get; set; }

        public Guid SubCategoryId { get; set; }
        
        [Required]
        public ToneEnum Tone { get; set; }
        
        [Required]
        public string TextTranscript { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Payload.Reports
{
    public class ReportCreatePayload
    {
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string VoiceLink { get; set; }
    }
}

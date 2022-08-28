using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Payload.Reports
{
    public class ReportAdminReviewPayload
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public bool IsTrue { get; set; }
    }
}

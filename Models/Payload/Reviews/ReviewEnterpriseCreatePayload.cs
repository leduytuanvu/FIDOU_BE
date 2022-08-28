using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Payload.Reviews
{
    public class ReviewEnterpriseCreatePayload
    {
        public Guid Id { get; set; }

        [MaxLength(500), Required]
        public string Content { get; set; }

        [Required]
        public decimal ReviewPoint { get; set; }
    }
}

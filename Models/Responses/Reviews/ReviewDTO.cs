using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Responses.Reviews
{
    public class ReviewDTO
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public decimal ReviewPoint { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Responses.Notifications
{
    public class NotifyBaseDTO
    {
        public Guid TargetAccountId { get; set; }
        
        public DateTime CreatedTime { get; set; }
        public bool IsRead { get; set; }
    }
}

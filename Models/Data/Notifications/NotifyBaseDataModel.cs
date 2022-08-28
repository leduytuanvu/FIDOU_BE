using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Data.Notifications
{
    public class NotifyBaseDataModel
    {
        public Guid TargetAccountId { get; set; }
        
        public DateTime CreatedTime { get; set; }
        public bool IsRead { get; set; }
    }
}

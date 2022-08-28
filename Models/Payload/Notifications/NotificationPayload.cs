using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Payload.Notifications
{
    public class NotificationPayload
    {
        public Guid AccountId { get; set; }

        [Required, MaxLength(500)]
        public string Content { get; set; }
    }
}

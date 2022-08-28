using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Payload.Wards
{
    public class WardCreatePayload
    {
        [Key, MaxLength(10)]
        public string Code { get; set; }

        [MaxLength(250), Required]
        public string Name { get; set; }

        [Required]
        public string DistrictCode { get; set; }
    }
}

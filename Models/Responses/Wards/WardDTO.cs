using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Responses.Wards
{
    public class WardDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
    }
}

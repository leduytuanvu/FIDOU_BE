﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities.Enums;

namespace VoiceAPI.Models.Responses.Jobs
{
    public class JobDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? DayDuration { get; set; }
        public int? HourDuration { get; set; }
        public int? MinuteDuration { get; set; }

        public Guid EnterpriseId { get; set; }

        public Guid SubCategoryId { get; set; }

        public JobStatusEnum JobStatus { get; set; }

        public decimal Price { get; set; }

        public bool IsPublic { get; set; }
        
        public ToneEnum Tone { get; set; }
        
        public DateTime CreatedTime { get; set; }
    }
}

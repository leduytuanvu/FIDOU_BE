using System;
using System.ComponentModel.DataAnnotations;
using VoiceAPI.Entities.Enums;

namespace VoiceAPI.Models.Data.Jobs
{
    public class JobUpdateDataModel
    {
        [Required]
        public Guid Id { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public int? DayDuration { get; set; }
        public int? HourDuration { get; set; }
        public int? MinuteDuration { get; set; }

        [Required]
        public Guid EnterpriseId { get; set; }

        public Guid? SubCategoryId { get; set; }

        public decimal? Price { get; set; }
        
        public ToneEnum? Tone { get; set; }
    }
}
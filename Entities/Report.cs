using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Entities
{
    [Table(nameof(Report))]
    public class Report
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string VoiceLink { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public bool IsReviewed { get; set; }

        public bool? IsTrue { get; set; }

        [ForeignKey(nameof(Id))]
        public Order Order { get; set; }
    }
}

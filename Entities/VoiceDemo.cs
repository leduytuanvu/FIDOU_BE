using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities.Enums;

namespace VoiceAPI.Entities
{
    [Table(nameof(VoiceDemo))]
    [Index(nameof(Url), IsUnique = true)]
    public class VoiceDemo
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid CandidateId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(500), Required]
        public string Url { get; set; }
        
        [Required]
        public ToneEnum Tone { get; set; }
        public string TextTranscript { get; set; }
        
        
        public Guid SubCategoryId { get; set; }

        [ForeignKey(nameof(CandidateId))]
        public Candidate Candidate { get; set; }

        [ForeignKey(nameof(SubCategoryId))]
        public SubCategory SubCategory { get; set; }
    }
}

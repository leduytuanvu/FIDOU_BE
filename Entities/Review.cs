using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoiceAPI.Entities
{
    [Table(nameof(Review))]
    public class Review
    {
        [Key]
        public Guid Id { get; set; }
        
        [MaxLength(500), Required]
        public string Content { get; set; }

        [Required]
        public decimal ReviewPoint { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }

        [ForeignKey(nameof(Id))]
        public Order Order { get; set; }
    }
}

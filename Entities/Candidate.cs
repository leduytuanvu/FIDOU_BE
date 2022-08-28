using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;

namespace VoiceAPI.Entities
{
    [Table(nameof(Candidate))]
    public class Candidate
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(5000)]
        public string Description { get; set; }

        [MaxLength(200), Required]
        public string Name { get; set; }
        public GenderEnum? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string AvatarUrl { get; set; }

        public AccentEnum Accent { get; set; }

        [MaxLength(15), Phone]
        public string PhoneContact { get; set; }

        [MaxLength(500), EmailAddress]
        public string EmailContact { get; set; }
        
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string LinkedinUrl { get; set; }

        public WorkingStatusEnum Status { get; set; }

        public string Province { get; set; }

        public List<string> SubCategorieNames { get; set; }
        

        [ForeignKey(nameof(Id))]
        public Account Account { get; set; }

        public List<JobInvitation> JobInvitations { get; set; }
        public List<FavouriteJob> FavouriteJob { get; set; }
        public List<VoiceDemo> VoiceDemos { get; set; }
        public List<ConversationSchedule> ConversationSchedules { get; set; }
    }
}
using System;
using System.Collections.Generic;
using VoiceAPI.Entities.Enums;
using VoiceAPI.Models.Responses.Reviews;
using VoiceAPI.Models.Responses.VoiceDemos;

namespace VoiceAPI.Models.Responses.Candidates
{
    public class CandidateGetProfileDTO
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
        public GenderEnum? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string AvatarUrl { get; set; }

        public AccentEnum Accent { get; set; }

        public string PhoneContact { get; set; }
        public string EmailContact { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string LinkedinUrl { get; set; }

        public WorkingStatusEnum Status { get; set; }

        public string ProvinceName { get; set; }

        public List<string> SubCategorieNames { get; set; }

        public List<VoiceDemoDTO> VoiceDemos { get; set; }
        
        public List<ReviewDTO> Reviews { get; set; }
        
        public decimal AverageReviewPoint { get; set; }
    }
}
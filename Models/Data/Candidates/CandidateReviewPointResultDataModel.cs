using VoiceAPI.Entities;

namespace VoiceAPI.Models.Data.Candidates
{
    public class CandidateReviewPointResultDataModel
    {
        public decimal ReviewPointAverage { get; set; }
        public Candidate Candidate { get; set; }
    }
}
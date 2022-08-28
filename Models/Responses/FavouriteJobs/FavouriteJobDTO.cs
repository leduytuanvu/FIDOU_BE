using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Responses.Jobs;

namespace VoiceAPI.Models.Responses.FavouriteJobs
{
    public class FavouriteJobDTO
    {
        public Guid Id { get; set; }

        public Guid CandidateId { get; set; }
        public JobDTO Job { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}

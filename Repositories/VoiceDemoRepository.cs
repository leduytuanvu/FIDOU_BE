using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.DbContextVoiceAPI;
using VoiceAPI.Entities;
using VoiceAPI.IRepositories;
using VoiceAPI.Repository;

namespace VoiceAPI.Repositories
{
    public class VoiceDemoRepository : BaseRepository<VoiceDemo>, IVoiceDemoRepository
    {
        public VoiceDemoRepository(VoiceAPIDbContext context) : base(context)
        {
        }
    }
}

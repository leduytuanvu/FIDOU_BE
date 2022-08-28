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
    public class WardRepository : BaseRepository<Ward>, IWardRepository
    {
        private readonly VoiceAPIDbContext _context;

        public WardRepository(VoiceAPIDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> RemoveAll()
        {
            try
            {
                _context.Wards.RemoveRange(_context.Set<Ward>().ToList());

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

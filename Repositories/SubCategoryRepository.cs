using Microsoft.EntityFrameworkCore;
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
    public class SubCategoryRepository : BaseRepository<SubCategory>, ISubCategoryRepository
    {
        private readonly VoiceAPIDbContext _context;

        public SubCategoryRepository(VoiceAPIDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Category> GetCategoryById(Guid categoryId)
        {
            var response = await _context.SubCategories.Join(_context.Categories,
                                            tempSubCategory => tempSubCategory.CategoryId,
                                            tempCategory => tempCategory.Id,
                                            (tempSubCategory, tempCategory) => new { tempSubCategory, tempCategory })
                                    .Where(result => result.tempCategory.Id.CompareTo(categoryId) == 0)
                                    .Select(result => result.tempCategory)
                                    .FirstOrDefaultAsync();

            return response;
        }
    }
}

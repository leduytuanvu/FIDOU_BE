﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.DbContextVoiceAPI;
using VoiceAPI.Entities;
using VoiceAPI.IRepository;

namespace VoiceAPI.IRepositories
{
    public interface ISubCategoryRepository : IBaseRepository<SubCategory>
    {
        Task<Category> GetCategoryById(Guid categoryId);
    }
}

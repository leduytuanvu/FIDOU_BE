using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.IRepository;

namespace VoiceAPI.IRepositories
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Task<Account> GetByPhoneNumberAndPassword(string phoneNumber, string password);
        Task<Account> GetByEmailAndPassword(string email, string password);
        Task<Account> GetByEmail(string email);
        Task<Enterprise> GetEnterpriseById(Guid id);
        Task<Candidate> GetCandidateById(Guid id);
    }
}

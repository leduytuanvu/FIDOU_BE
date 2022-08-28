using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.DbContextVoiceAPI;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;
using VoiceAPI.IRepositories;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Candidates;
using VoiceAPI.Repository;

namespace VoiceAPI.Repositories
{
    public class CandidateRepository : BaseRepository<Candidate>, ICandidateRepository
    {
        private readonly VoiceAPIDbContext _context;

        public CandidateRepository(VoiceAPIDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Category> GetCategoryById(Guid? categoryId)
        {
            var response = await _context.Categories
                            .AsNoTracking()
                            .FirstOrDefaultAsync(tempCategory => tempCategory.Id.CompareTo(categoryId) == 0);

            return response;
        }

        public async Task<List<Order>> GetOrderHistory(Guid id, OrderStatusEnum? orderStatus)
        {
            var response = await _context.Candidates.Join(_context.Orders,
                                                candidate => candidate.Id,
                                                order => order.CandidateId,
                                                (candidate, order) => new { candidate, order })
                                        .Where(result => result.candidate.Id.CompareTo(id) == 0)
                                        .Select(result => result.order)
                                        .ToListAsync();

            if (orderStatus != null)
            {
                response = response.AsEnumerable()
                    .Where(tempOrder => tempOrder.Status == orderStatus)
                    .ToList();
            }

            return response;
        }

        public async Task<Job> GetJobByOrderId(Guid orderId)
        {
            var job = await _context.Candidates.Join(_context.Orders,
                    candidate => candidate.Id,
                    order => order.CandidateId,
                    (candidate, order) => new { candidate, order })
                .Select(result => result.order).Join(_context.Jobs,
                    order => order.JobId,
                    job => job.Id,
                    (order, job) => new { order, job })
                .Where(result => result.order.Id.CompareTo(orderId) == 0)
                .Select(result => result.job)
                .FirstOrDefaultAsync();

            return job;
        }

        public async Task<List<Review>> GetReviewsByCandidateId(Guid id)
        {
            var reviews = await _context.Candidates.Join(_context.Orders,
                    candidate => candidate.Id,
                    order => order.CandidateId,
                    (candidate, order) => new { candidate, order })
                .Where(result => result.candidate.Id.CompareTo(id) == 0)
                .Select(result => result.order).Join(_context.Reviews,
                    order => order.Id,
                    review => review.Id,
                    (order, review) => new { order, review })
                .Select(result => result.review)
                .ToListAsync();

            return reviews;
        }

        public async Task<GenericResult<CandidateReviewPointResultDataModel>> GetAveragePointOfCandidate(Guid id)
        {
            var reviews = await _context.Candidates.Join(_context.Orders,
                    candidate => candidate.Id,
                    order => order.CandidateId,
                    (candidate, order) => new { candidate, order })
                .Where(result => result.candidate.Id.CompareTo(id) == 0)
                .Select(result => result.order).Join(_context.Reviews,
                    order => order.Id,
                    review => review.Id,
                    (order, review) => new { order, review })
                .Select(result => result.review)
                .ToListAsync();
            
            var points = reviews.AsEnumerable()
                .Select(tempReview => tempReview.ReviewPoint)
                .ToList();

            if (points.Count == 0)
                return null;

            var totalPoints = CountTotal(points, 0, decimal.Zero);

            var averagePoint = decimal.Divide(totalPoints, Convert.ToDecimal(points.Count));

            var targetCandidate = await GetById(id);
            
            return GenericResult<CandidateReviewPointResultDataModel>.Success(new CandidateReviewPointResultDataModel
            {
                ReviewPointAverage = averagePoint, 
                Candidate = targetCandidate
            });
        }

        public async Task<Candidate> UpdateCandidateStatusAfterReview(Candidate candidate)
        {
            Update(candidate);
            await SaveAsync();

            return candidate;
        }

        private static decimal CountTotal(List<decimal> points ,int index, decimal total)
        {
            if (index == points.Count)
                return total;

            total = decimal.Add(total, points.ElementAt(index));

            return CountTotal(points, index + 1, total);
        }
        
        public async Task<List<VoiceDemo>> GetVoiceDemosByCandidateId(Guid id)
        {
            var voiceDemos = await _context.Candidates.Join(_context.VoiceDemos,
                                                    candidate => candidate.Id,
                                                    voiceDemo => voiceDemo.CandidateId,
                                                    (candidate, voiceDemo) => new { candidate, voiceDemo })
                                            .Where(result => result.candidate.Id.CompareTo(id) == 0)
                                            .Select(result => result.voiceDemo)
                                            .ToListAsync();

            return voiceDemos;
        }
    }
}

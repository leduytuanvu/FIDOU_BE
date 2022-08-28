using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Helpers;
using VoiceAPI.IRepositories;
using VoiceAPI.IRepository;
using VoiceAPI.IServices;
using VoiceAPI.Repositories;
using VoiceAPI.Repository;
using VoiceAPI.Services;

namespace VoiceAPI.DI
{
    public static class ServiceDI
    {
        public static void ConfigServiceDI(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            //Tools
            services.AddScoped<ITools, Tools>();

            // Auth
            services.AddScoped<IAuthService, AuthService>();

            // Account
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountService, AccountService>();

            // Admin
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IAdminService, AdminService>();

            // Candidate
            services.AddScoped<ICandidateRepository, CandidateRepository>();
            services.AddScoped<ICandidateService, CandidateService>();

            // Category
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();

            // Conversation Schedule
            services.AddScoped<IConversationScheduleRepository, ConversationScheduleRepository>();
            services.AddScoped<IConversationScheduleService, ConversationScheduleService>();

            // District
            services.AddScoped<IDistrictRepository, DistrictRepository>();
            services.AddScoped<IDistrictService, DistrictService>();

            // Enterprise
            services.AddScoped<IEnterpriseRepository, EnterpriseRepository>();
            services.AddScoped<IEnterpriseService, EnterpriseService>();

            // Favourite Job
            services.AddScoped<IFavouriteJobRepository, FavouriteJobRepository>();
            services.AddScoped<IFavouriteJobService, FavouriteJobService>();

            // Job
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobService, JobService>();

            // Job Invitation
            services.AddScoped<IJobInvitationRepository, JobInvitationRepository>();
            services.AddScoped<IJobInvitationService, JobInvitationService>();

            // Notification
            services.AddScoped<INotificationService, NotificationService>();

            // Order
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();

            // Province
            services.AddScoped<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<IProvinceService, ProvinceService>();

            // Report
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportService, ReportService>();

            // Review
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IReviewService, ReviewService>();

            // SubCategory
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<ISubCategoryService, SubCategoryService>();

            // Transaction History
            services.AddScoped<ITransactionHistoryRepository, TransactionHistoryRepository>();
            services.AddScoped<ITransactionHistoryService, TransactionHistoryService>();

            // Voice Demo
            services.AddScoped<IVoiceDemoRepository, VoiceDemoRepository>();
            services.AddScoped<IVoiceDemoService, VoiceDemoService>();

            // Wallet
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IWalletService, WalletService>();

            // Ward
            services.AddScoped<IWardRepository, WardRepository>();
            services.AddScoped<IWardService, WardService>();
        }
    }
}
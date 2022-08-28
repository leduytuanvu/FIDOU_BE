using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VoiceAPI.Entities;
using VoiceAPI.IRepositories;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Reviews;
using VoiceAPI.Models.Payload.Reviews;
using VoiceAPI.Models.Responses.Reviews;

namespace VoiceAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IMapper _mapper;

        private readonly IReviewRepository _reviewRepository;

        private readonly IOrderService _orderService;

        public ReviewService(IMapper mapper, 
            IReviewRepository reviewRepository, 
            IOrderService orderService)
        {
            _mapper = mapper;

            _reviewRepository = reviewRepository;

            _orderService = orderService;
        }

        public async Task<GenericResult<ReviewDTO>> EnterpriseCreateReview(ReviewEnterpriseCreateDataModel dataModel)
        {
            var targetOrder = await _orderService.GetById(dataModel.Payload.Id);

            if (targetOrder.Data == null)
                return GenericResult<ReviewDTO>.Error((int)HttpStatusCode.NotFound,
                                        "Order is not found.");

            if (targetOrder.Data.Status != Entities.Enums.OrderStatusEnum.FINISHED)
                return GenericResult<ReviewDTO>.Error((int)HttpStatusCode.BadRequest, 
                                        "V400_38", 
                                        "Công việc phải kết thúc mới được đánh giá.");

            if (!(await _orderService.IsOrderBelongToEnterprise(dataModel.Payload.Id, dataModel.EnterpriseId)).Data)
                return GenericResult<ReviewDTO>.Error((int)HttpStatusCode.BadRequest,
                                        "V400_36",
                                        "Order is not belong to any Job of this Enterprise.");

            var targetReview = await _reviewRepository.GetById(dataModel.Payload.Id);

            if (targetReview != null)
                return GenericResult<ReviewDTO>.Error((int)HttpStatusCode.BadRequest,
                                        "V400_37",
                                        "Đã đánh giá.");

            targetReview = _mapper.Map<Review>(dataModel.Payload);

            targetReview.CreatedTime = DateTime.UtcNow;

            _reviewRepository.Create(targetReview);
            await _reviewRepository.SaveAsync();

            var response = _mapper.Map<ReviewDTO>(targetReview);

            return GenericResult<ReviewDTO>.Success(response);
        }
    }
}

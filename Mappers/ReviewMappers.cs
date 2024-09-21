using OloEcomm.Data.Enum;
using OloEcomm.Dtos.Review;
using OloEcomm.Model;

namespace OloEcomm.Mappers
{
    public static class ReviewMappers
    {
        public static ReviewDto ToReviewDto(this Review reviewModel)
        {
            return new ReviewDto
            {
                Id = reviewModel.Id,
                User = reviewModel.User,
                Comment = reviewModel.Comment,
                Rating = reviewModel.Rating,
                ReviewDate = reviewModel.ReviewDate,
                ProductId = reviewModel.ProductId,
            };
        }

        public static Review ToCreateReviewDto(this CreateReviewDto createReviewDto, int productId, Rating rating)
        {
            return new Review
            {
                Comment = createReviewDto.Comment,
                 ProductId = productId,
                 Rating = rating,

            };
        }

        public static Review ToUpdateReviewDto(this UpdateReviewDto updateReviewDto, Rating rating)
        {
            return new Review
            {
                Comment = updateReviewDto.Comment,
                Rating = rating,
            };
        }
    }
}

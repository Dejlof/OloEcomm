using OloEcomm.Dtos.Review;
using OloEcomm.Model;

namespace OloEcomm.Mappers
{
    public static class WishlistMapper
    {
        public static ReviewDto ToWishlistDto(this Review reviewModel)
        {
            return new ReviewDto
            {
                Id = reviewModel.Id,
                User = reviewModel.User,
                ReviewDate = reviewModel.ReviewDate,
                ProductId = reviewModel.ProductId,

            };
        }
    }
}
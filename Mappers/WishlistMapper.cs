using OloEcomm.Dtos.Wishlist;
using OloEcomm.Model;

namespace OloEcomm.Mappers
{
    public static class WishlistMapper
    {
        public static WishListDto ToWishlistDto(this Wishlist wishlistModel)
        {
            return new WishListDto
            {
               Id = wishlistModel.Id,
               CreatedDate = wishlistModel.CreatedDate,
               ProductId = wishlistModel.ProductId,
            };
        }

        public static Wishlist CreateToWishlistDto(this CreateWishlistDto wishlistDto, int productId )
        {
            return new Wishlist
            {
                ProductId = productId,
            };
        }
    }
}
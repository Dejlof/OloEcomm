﻿using OloEcomm.Dtos.Cart;
using OloEcomm.Model;

namespace OloEcomm.Mappers
{
    public static class CartMappers
    {
       public static CartItemDto ToCartItemDto(this CartItem cartItem)
        {
            return new CartItemDto
            {
               ProductAdded = cartItem.ProductAdded,
                Quantity = cartItem.Quantity,
                ProductId = cartItem.ProductId,
                OrderedBy = cartItem.User?.UserName

            };

        }

        public static CartItem ToCreateCartItemDto(this CreateCartItemDto createCartItem, int productId)
        {
            return new CartItem
            {
                Quantity = createCartItem.Quantity,
                ProductId = productId,
            };
        }

        public static CartItem ToUpdateCartItemDto(this UpdateCartItemDto updateCartItem)
        {
            return new CartItem
            {
                Quantity = updateCartItem.Quantity,
               
            };
        }
    }
}

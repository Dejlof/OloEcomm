﻿using OloEcomm.Dtos.Product;

namespace OloEcomm.Dtos.Cart
{
    public class CartItemDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public int ShoppingCartId { get; set; }
    }
}
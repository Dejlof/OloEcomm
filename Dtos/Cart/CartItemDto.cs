﻿using OloEcomm.Dtos.Product;

namespace OloEcomm.Dtos.Cart
{
    public class CartItemDto
    {
     

        public int ProductId { get; set; }

        public int Quantity { get; set; } = 1;

        public string? ProductAdded { get; set; }

        public string OrderedBy { get; set; } = string.Empty;


    }
}

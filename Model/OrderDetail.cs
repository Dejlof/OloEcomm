﻿using System.ComponentModel.DataAnnotations.Schema;

namespace OloEcomm.Model
{
    public class OrderDetail
    {
        public int Id { get; set; }


        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public int OrderId { get; set; }
       
        public Order Order { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public string ProductName { get; set; } = string.Empty;
    }
}

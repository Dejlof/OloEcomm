﻿using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations.Schema;

namespace OloEcomm.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } = decimal.Zero;
        public int CategoryId { get; set; }
      
        public Category Category { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
       
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();


    }
}
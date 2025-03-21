using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OloEcomm.Helpers
{
    public class ProductQuery
    {
        public string? Search { get; set; } = null;
        public decimal? MinPrice { get; set; } = null;
        public decimal? MaxPrice { get; set; } = null;
    }
}
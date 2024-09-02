using OloEcomm.Dtos.Review;

using System.ComponentModel.DataAnnotations.Schema;

namespace OloEcomm.Dtos.Product
{
    public class ProductDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = decimal.Zero;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public int? CategoryId { get; set; }

       public List<ReviewDto>? Reviews { get; set; }
    }
}

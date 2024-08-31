using OloEcomm.Data.Enum;

namespace OloEcomm.Model
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string Comment { get; set; }
        public Rating Rating { get; set; } 
        public DateTime ReviewDate { get; set; } = DateTime.Now;
        public Product Product { get; set; }
    }
}

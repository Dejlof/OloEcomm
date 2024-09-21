using OloEcomm.Data.Enum;

namespace OloEcomm.Model
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string User { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty ;
        public Rating Rating { get; set; } 
        public DateTime ReviewDate { get; set; } = DateTime.Now;
        public Product Product { get; set; }
    }
}

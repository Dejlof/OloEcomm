using OloEcomm.Data.Enum;

namespace OloEcomm.Dtos.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public Rating Rating { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.Now;
        public int ProductId { get; set; }

    }
}

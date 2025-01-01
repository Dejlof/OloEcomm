namespace OloEcomm.Model
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public Product? Product { get; set; }
        public string? UserId { get; set; }

        public User? User { get; set; }

        public string UserWishlist { get; set; } = string.Empty;   

        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}

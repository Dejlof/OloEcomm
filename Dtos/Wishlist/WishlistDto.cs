namespace OloEcomm.Dtos.Wishlist
{
    public class WishListDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

    

        public string User { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

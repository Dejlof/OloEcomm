namespace OloEcomm.Model
{
    public class CartItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public string? UserId { get; set; }

        public User? User { get; set; }


        public int Quantity {get; set; } = 1;

        public string OrderedBy { get; set; } = string.Empty;


    }
}

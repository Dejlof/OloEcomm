namespace OloEcomm.Model
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public string User {  get; set; } = string.Empty;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    }
}

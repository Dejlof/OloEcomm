namespace OloEcomm.Model
{
    public class CartItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }


        public int Quantity {get; set; } = 1;

        public string User { get; set; } = string.Empty;


    }
}

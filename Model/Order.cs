namespace OloEcomm.Model
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string OrderedBy { get; set; } = string.Empty;
        public int AddressId { get; set; } 
        public Address Address { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();   
        public Payment Payment { get; set; }
    }
}

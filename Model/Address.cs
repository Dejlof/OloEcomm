namespace OloEcomm.Model
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; } = new List<Order>(); 
    }
}

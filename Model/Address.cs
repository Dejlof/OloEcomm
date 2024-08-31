namespace OloEcomm.Model
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string CreatedBy { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; }
    }
}

﻿namespace OloEcomm.Dtos.Address
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
    }
}

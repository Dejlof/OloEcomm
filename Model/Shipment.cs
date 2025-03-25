using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OloEcomm.Model
{
    public class Shipment
    {
        public int Id { get; set; }
        public int OrderDetailId { get; set; }
        public string TrackingNumber { get; set; } = Guid.NewGuid().ToString();
        public DateTime ShippedDate { get; set; } 
        public DateTime? DeliveredDate { get; set; }

       public OrderDetail OrderDetail { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Receiving.Models
{
    public class ReceivingDetails
    {
        public int PurchaseOrderNumber { get; set; }
        public string Vendor { get; set; }
        public string Phone { get; set; }
        public List<ReceivingItem> ReceivingItems { get; set; }
    }
}

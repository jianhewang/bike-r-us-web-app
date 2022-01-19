using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Receiving.Models
{
    public class ReceivingItem
    {
        public int PurchaseOrderDetailId { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int OrdQty { get; set; }
        public int RecQty { get; set; }

    }
}

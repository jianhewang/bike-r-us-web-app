using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Purchasing.Models
{
    /// <summary>
    /// Represents information of a part
    /// </summary>
    public class PurchaseOrderItem
    {
        public int PurchaseOrderDetailID { get; set; }
        public int ID { get; set; }
        public string Description { get; set; }
        public int QtyOnHand { get; set; }
        public int ReorderLevel { get; set; }
        public int QtyOnOrder { get; set; }
        public int QtyToOrder { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}

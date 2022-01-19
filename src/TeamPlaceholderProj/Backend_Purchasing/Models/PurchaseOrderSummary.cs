using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Purchasing.Models
{
    /// <summary>
    /// Represents information of a purchase order, including vendor info, order summary, and order items 
    /// </summary>
    public class PurchaseOrderSummary
    {
        public string Phone { get; set; }
        public string City { get; set; }
        public int PurchaseOrderNumber { get; set; }
        public Decimal TaxAmount { get; set; }
        public Decimal SubTotal { get; set; }
        public List<PurchaseOrderItem> PurchaseOrderDetails { get; set; }

        public PurchaseOrderSummary()
        {

        }

        public PurchaseOrderSummary(string phone, string city, int poNumber, decimal gst, decimal subtotal, List<PurchaseOrderItem> orderitems)
        {
            Phone = phone;
            City = city;
            PurchaseOrderNumber = poNumber;
            TaxAmount = gst;
            SubTotal = subtotal;
            PurchaseOrderDetails = orderitems;
        }
    }
}

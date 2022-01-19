using System.ComponentModel.DataAnnotations;

namespace Backend_SalesReturns.Models
{
    public class Parts
    {
        //general + showing parts on sales
        [Key]
        public int PartID { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal SellingPrice { get; set; }

    }


    // for refund, can work 
    public class RefundParts : Parts
    {
        public int SaleRefundID {get; set;}
        public int SaleDetailID { get; set; }
        public bool Refundable { get; set; }
        public int RefundQuantity { get; set; }
        public int ToRefund { get; set; }
        public string RefundReason { get; set; }
    }
    // saving into SalePart
    public class SaleParts : Parts
    {
        public int ToBuy { get; set; }
    }

    
}
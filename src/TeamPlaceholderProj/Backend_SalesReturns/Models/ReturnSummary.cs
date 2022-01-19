using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_SalesReturns.Models
{
    public class ReturnSummary
    {
        [Key]
        public int SaleRefundID { get; set; }
        public int SaleID { get; set; }
        public List<RefundParts> PartsReturned { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SubTotal { get; set; }
        public Coupon OriginalCoupon { get; set; }

        public ReturnSummary()
        {
            PartsReturned = new List<RefundParts>();
        }
    }
}

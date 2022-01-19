using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_SalesReturns.Models
{
    public class ForRefundSalesSummary
    {
        public int SaleID { get; set; }
        public List<RefundParts> PartsList { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SubTotal { get; set; }
        public int? DiscountAmount { get; set; }
        public int? CouponID { get; set; }
    }
}

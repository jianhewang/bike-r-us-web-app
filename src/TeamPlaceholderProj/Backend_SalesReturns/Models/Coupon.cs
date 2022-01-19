using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_SalesReturns.Models
{
    public class Coupon
    {
        public int CouponID { get; set; }
        public string CouponIDValue { get; set; }
        public int CouponDiscount { get; set; }
    }
}

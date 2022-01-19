using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend_SalesReturns.Models
{
    public class SalesSummary
    {
        [Required]
        public int SaleID { get; set; }
        [Required]
        public List<Parts> PartsList { get; set; }
        public int EmployeeID { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SubTotal { get; set; }
        [Required]
        public string PaymentType { get; set; }
        public Coupon Coupon { get; set; }

        public SalesSummary()
        {
            PartsList = new List<Parts>();
        }

    }
}
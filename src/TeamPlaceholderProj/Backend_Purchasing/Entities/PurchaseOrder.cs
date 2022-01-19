﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Backend_Purchasing.Entities
{
    internal partial class PurchaseOrder
    {
        public PurchaseOrder()
        {
            PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
        }

        [Key]
        [Column("PurchaseOrderID")]
        public int PurchaseOrderId { get; set; }
        public int PurchaseOrderNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? OrderDate { get; set; }
        [Column(TypeName = "money")]
        public decimal TaxAmount { get; set; }
        [Column(TypeName = "money")]
        public decimal SubTotal { get; set; }
        public bool Closed { get; set; }
        [StringLength(100)]
        public string Notes { get; set; }
        [Column("EmployeeID")]
        public int EmployeeId { get; set; }
        [Column("VendorID")]
        public int VendorId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [InverseProperty("PurchaseOrders")]
        public virtual Employee Employee { get; set; }
        [ForeignKey(nameof(VendorId))]
        [InverseProperty("PurchaseOrders")]
        public virtual Vendor Vendor { get; set; }
        [InverseProperty(nameof(PurchaseOrderDetail.PurchaseOrder))]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    }
}
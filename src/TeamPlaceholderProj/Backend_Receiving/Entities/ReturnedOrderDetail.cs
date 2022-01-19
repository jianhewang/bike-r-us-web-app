﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Backend_Receiving.Entities
{
    internal partial class ReturnedOrderDetail
    {
        [Key]
        [Column("ReturnedOrderDetailID")]
        public int ReturnedOrderDetailId { get; set; }
        [Column("ReceiveOrderID")]
        public int ReceiveOrderId { get; set; }
        [Column("PurchaseOrderDetailID")]
        public int? PurchaseOrderDetailId { get; set; }
        [StringLength(50)]
        public string ItemDescription { get; set; }
        public int Quantity { get; set; }
        [Required]
        [StringLength(50)]
        public string Reason { get; set; }
        [StringLength(50)]
        public string VendorPartNumber { get; set; }

        [ForeignKey(nameof(PurchaseOrderDetailId))]
        [InverseProperty("ReturnedOrderDetails")]
        public virtual PurchaseOrderDetail PurchaseOrderDetail { get; set; }
        [ForeignKey(nameof(ReceiveOrderId))]
        [InverseProperty("ReturnedOrderDetails")]
        public virtual ReceiveOrder ReceiveOrder { get; set; }
    }
}
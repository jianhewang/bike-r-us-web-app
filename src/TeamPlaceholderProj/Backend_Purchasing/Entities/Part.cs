﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Backend_Purchasing.Entities
{
    internal partial class Part
    {
        public Part()
        {
            PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
        }

        [Key]
        [Column("PartID")]
        public int PartId { get; set; }
        [Required]
        [StringLength(40)]
        public string Description { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal PurchasePrice { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal SellingPrice { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
        public int QuantityOnOrder { get; set; }
        [Column("CategoryID")]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(1)]
        public string Refundable { get; set; }
        public bool Discontinued { get; set; }
        [Column("VendorID")]
        public int VendorId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        [InverseProperty("Parts")]
        public virtual Category Category { get; set; }
        [ForeignKey(nameof(VendorId))]
        [InverseProperty("Parts")]
        public virtual Vendor Vendor { get; set; }
        [InverseProperty(nameof(PurchaseOrderDetail.Part))]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    }
}
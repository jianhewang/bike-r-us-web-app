// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Backend_Receiving.Entities
{
    [Index(nameof(SaleId), nameof(PartId), Name = "UQ_SaleDetails_SaleIDPartID", IsUnique = true)]
    internal partial class SaleDetail
    {
        [Key]
        [Column("SaleDetailID")]
        public int SaleDetailId { get; set; }
        [Column("SaleID")]
        public int SaleId { get; set; }
        [Column("PartID")]
        public int PartId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal SellingPrice { get; set; }

        [ForeignKey(nameof(PartId))]
        [InverseProperty("SaleDetails")]
        public virtual Part Part { get; set; }
        [ForeignKey(nameof(SaleId))]
        [InverseProperty("SaleDetails")]
        public virtual Sale Sale { get; set; }
    }
}
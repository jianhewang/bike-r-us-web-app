// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Backend_Purchasing.Entities
{
    internal partial class Category
    {
        public Category()
        {
            Parts = new HashSet<Part>();
        }

        [Key]
        [Column("CategoryID")]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(40)]
        public string Description { get; set; }

        [InverseProperty(nameof(Part.Category))]
        public virtual ICollection<Part> Parts { get; set; }
    }
}
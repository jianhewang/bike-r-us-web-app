﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Backend_Receiving.Entities
{
    internal partial class StandardJob
    {
        [Key]
        [Column("StandardJobID")]
        public int StandardJobId { get; set; }
        [Required]
        [StringLength(100)]
        public string Description { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal StandardHours { get; set; }
    }
}
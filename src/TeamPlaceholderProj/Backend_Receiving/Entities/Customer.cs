﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Backend_Receiving.Entities
{
    internal partial class Customer
    {
        public Customer()
        {
            CustomerVehicles = new HashSet<CustomerVehicle>();
        }

        [Key]
        [Column("CustomerID")]
        public int CustomerId { get; set; }
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }
        [StringLength(40)]
        public string Address { get; set; }
        [StringLength(20)]
        public string City { get; set; }
        [StringLength(2)]
        public string Province { get; set; }
        [StringLength(6)]
        public string PostalCode { get; set; }
        [Required]
        [StringLength(12)]
        public string ContactPhone { get; set; }
        public bool Textable { get; set; }
        [StringLength(30)]
        public string EmailAddress { get; set; }

        [InverseProperty(nameof(CustomerVehicle.Customer))]
        public virtual ICollection<CustomerVehicle> CustomerVehicles { get; set; }
    }
}
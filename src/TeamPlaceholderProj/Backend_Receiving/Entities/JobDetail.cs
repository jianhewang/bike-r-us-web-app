// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Backend_Receiving.Entities
{
    internal partial class JobDetail
    {
        public JobDetail()
        {
            JobDetailParts = new HashSet<JobDetailPart>();
        }

        [Key]
        [Column("JobDetailID")]
        public int JobDetailId { get; set; }
        [Column("JobID")]
        public int JobId { get; set; }
        [Required]
        [StringLength(100)]
        public string Description { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal JobHours { get; set; }
        public string Comments { get; set; }
        [Column("CouponID")]
        public int? CouponId { get; set; }
        [Required]
        [StringLength(1)]
        public string StatusCode { get; set; }
        [Column("EmployeeID")]
        public int? EmployeeId { get; set; }

        [ForeignKey(nameof(CouponId))]
        [InverseProperty("JobDetails")]
        public virtual Coupon Coupon { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        [InverseProperty("JobDetails")]
        public virtual Employee Employee { get; set; }
        [ForeignKey(nameof(JobId))]
        [InverseProperty("JobDetails")]
        public virtual Job Job { get; set; }
        [InverseProperty(nameof(JobDetailPart.JobDetail))]
        public virtual ICollection<JobDetailPart> JobDetailParts { get; set; }
    }
}
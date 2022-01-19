﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Backend_Receiving.Entities;

#nullable disable

namespace Backend_Receiving.DAL
{
    internal partial class Ebike_DMIT2018Context : DbContext
    {
        public Ebike_DMIT2018Context()
        {
        }

        public Ebike_DMIT2018Context(DbContextOptions<Ebike_DMIT2018Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerVehicle> CustomerVehicles { get; set; }
        public virtual DbSet<DatabaseVersion> DatabaseVersions { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobDetail> JobDetails { get; set; }
        public virtual DbSet<JobDetailPart> JobDetailParts { get; set; }
        public virtual DbSet<Part> Parts { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual DbSet<ReceiveOrder> ReceiveOrders { get; set; }
        public virtual DbSet<ReceiveOrderDetail> ReceiveOrderDetails { get; set; }
        public virtual DbSet<ReturnedOrderDetail> ReturnedOrderDetails { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SaleDetail> SaleDetails { get; set; }
        public virtual DbSet<SaleRefund> SaleRefunds { get; set; }
        public virtual DbSet<SaleRefundDetail> SaleRefundDetails { get; set; }
        public virtual DbSet<StandardJob> StandardJobs { get; set; }
        public virtual DbSet<UnorderedPurchaseItemCart> UnorderedPurchaseItemCarts { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.ContactPhone)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.EmailAddress).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Province)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<CustomerVehicle>(entity =>
            {
                entity.HasKey(e => e.VehicleIdentification)
                    .HasName("PK_CustomerVehicles_VehicleIdentification");

                entity.Property(e => e.VehicleIdentification).IsFixedLength(true);

                entity.Property(e => e.Make).IsFixedLength(true);

                entity.Property(e => e.Model).IsFixedLength(true);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerVehicles)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerVehiclesCustomers_CustomerID");
            });

            modelBuilder.Entity<DatabaseVersion>(entity =>
            {
                entity.Property(e => e.DateTime).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.ContactPhone).IsUnicode(false);

                entity.Property(e => e.EmailAddress).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Province)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.SocialInsuranceNumber)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.PositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeesPositions_PositionID");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(e => e.VehicleIdentification).IsFixedLength(true);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobsEmployees_EmployeeID");

                entity.HasOne(d => d.VehicleIdentificationNavigation)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.VehicleIdentification)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobsCustomerVehicles_VehicleIdentification");
            });

            modelBuilder.Entity<JobDetail>(entity =>
            {
                entity.Property(e => e.StatusCode)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('I')")
                    .IsFixedLength(true);

                entity.HasOne(d => d.Coupon)
                    .WithMany(p => p.JobDetails)
                    .HasForeignKey(d => d.CouponId)
                    .HasConstraintName("FK_JobDetailsCoupons_CouponID");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.JobDetails)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_JobDetailsEmployees_EmployeeID");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobDetails)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobDetailsJobs_JobID");
            });

            modelBuilder.Entity<JobDetailPart>(entity =>
            {
                entity.HasOne(d => d.JobDetail)
                    .WithMany(p => p.JobDetailParts)
                    .HasForeignKey(d => d.JobDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobDetailPartsJobDetails_JobDetailID");

                entity.HasOne(d => d.Part)
                    .WithMany(p => p.JobDetailParts)
                    .HasForeignKey(d => d.PartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobDetailPartsParts_PartID");
            });

            modelBuilder.Entity<Part>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Refundable)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Y')")
                    .IsFixedLength(true);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Parts)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartsCategories_CategoryID");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.Parts)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartsVendors_VendorID");
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.PurchaseOrders)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseOrdersEmployees_EmployeeID");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.PurchaseOrders)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseOrdersVednors_VendorID");
            });

            modelBuilder.Entity<PurchaseOrderDetail>(entity =>
            {
                entity.HasOne(d => d.Part)
                    .WithMany(p => p.PurchaseOrderDetails)
                    .HasForeignKey(d => d.PartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseOrderDetailsParts_PartID");

                entity.HasOne(d => d.PurchaseOrder)
                    .WithMany(p => p.PurchaseOrderDetails)
                    .HasForeignKey(d => d.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseOrderDetailsPurchaseOrders_PurchaseOrderID");
            });

            modelBuilder.Entity<ReceiveOrder>(entity =>
            {
                entity.HasOne(d => d.PurchaseOrder)
                    .WithMany(p => p.ReceiveOrders)
                    .HasForeignKey(d => d.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReceiveOrdersPurchaseOrders_PurchaseOrderID");
            });

            modelBuilder.Entity<ReceiveOrderDetail>(entity =>
            {
                entity.HasOne(d => d.PurchaseOrderDetail)
                    .WithMany(p => p.ReceiveOrderDetails)
                    .HasForeignKey(d => d.PurchaseOrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReceiveOrderDetailsPurchaseOrderDetails_OrderDetailID");

                entity.HasOne(d => d.ReceiveOrder)
                    .WithMany(p => p.ReceiveOrderDetails)
                    .HasForeignKey(d => d.ReceiveOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReceiveOrderDetailsReceiveOrders_ReceiveOrderID");
            });

            modelBuilder.Entity<ReturnedOrderDetail>(entity =>
            {
                entity.HasOne(d => d.PurchaseOrderDetail)
                    .WithMany(p => p.ReturnedOrderDetails)
                    .HasForeignKey(d => d.PurchaseOrderDetailId)
                    .HasConstraintName("FK_ReturnedOrderDetailsPurchaseOrderDetails_OrderDetailID");

                entity.HasOne(d => d.ReceiveOrder)
                    .WithMany(p => p.ReturnedOrderDetails)
                    .HasForeignKey(d => d.ReceiveOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReturnedOrderDetailsReceiveOrders_ReceiveOrderID");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.Property(e => e.PaymentType).IsFixedLength(true);

                entity.Property(e => e.SaleDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Coupon)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.CouponId)
                    .HasConstraintName("FK_SalesCoupons_CouponID");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SalesEmployees_EmployeeID");
            });

            modelBuilder.Entity<SaleDetail>(entity =>
            {
                entity.HasOne(d => d.Part)
                    .WithMany(p => p.SaleDetails)
                    .HasForeignKey(d => d.PartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SaleDetailsParts_PartID");

                entity.HasOne(d => d.Sale)
                    .WithMany(p => p.SaleDetails)
                    .HasForeignKey(d => d.SaleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SaleDetailsSalesSaleID");
            });

            modelBuilder.Entity<SaleRefund>(entity =>
            {
                entity.Property(e => e.SaleRefundDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.SaleRefunds)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SaleRefundsEmployees_EmployeeID");

                entity.HasOne(d => d.Sale)
                    .WithMany(p => p.SaleRefunds)
                    .HasForeignKey(d => d.SaleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CK_SaleRefundsSales_SaleID");
            });

            modelBuilder.Entity<SaleRefundDetail>(entity =>
            {
                entity.HasOne(d => d.Part)
                    .WithMany(p => p.SaleRefundDetails)
                    .HasForeignKey(d => d.PartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SaleRefundDetailsParts_PartId");

                entity.HasOne(d => d.SaleRefund)
                    .WithMany(p => p.SaleRefundDetails)
                    .HasForeignKey(d => d.SaleRefundId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SaleRefundDetailsSaleRefunds_SaleRefundID");
            });

            modelBuilder.Entity<StandardJob>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);
            });

            modelBuilder.Entity<UnorderedPurchaseItemCart>(entity =>
            {
                entity.HasKey(e => e.CartId)
                    .HasName("PK__Unordere__51BCD797C3D9C762");
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.Property(e => e.PostalCode).IsFixedLength(true);

                entity.Property(e => e.ProvinceId)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('AB')")
                    .IsFixedLength(true);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
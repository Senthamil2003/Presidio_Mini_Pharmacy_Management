﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PharmacyManagementApi.Context;

#nullable disable

namespace PharmacyManagementApi.Migrations
{
    [DbContext(typeof(PharmacyContext))]
    [Migration("20240718155736_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.30")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("PharmacyManagementApi.Models.Brand", b =>
                {
                    b.Property<int>("BrandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BrandId"), 1L, 1);

                    b.Property<string>("BrandName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("BrandId");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Cart", b =>
                {
                    b.Property<int>("CartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartId"), 1L, 1);

                    b.Property<double>("Cost")
                        .HasColumnType("float");

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("MedicineId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<double>("TotalCost")
                        .HasColumnType("float");

                    b.HasKey("CartId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("MedicineId");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"), 1L, 1);

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustomerId"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsSubcribed")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CustomerId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.DeliveryDetail", b =>
                {
                    b.Property<int>("DeliveryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DeliveryId"), 1L, 1);

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("MedicineId")
                        .HasColumnType("int");

                    b.Property<int>("OrderDetailId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("DeliveryId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("MedicineId");

                    b.HasIndex("OrderDetailId");

                    b.ToTable("DeliveryDetails");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Feedback", b =>
                {
                    b.Property<int>("FeedbackId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FeedbackId"), 1L, 1);

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("FeedbackMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FeedbackTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MedicineId")
                        .HasColumnType("int");

                    b.Property<double>("Rating")
                        .HasColumnType("float");

                    b.HasKey("FeedbackId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("MedicineId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Medication", b =>
                {
                    b.Property<int>("MedicationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MedicationId"), 1L, 1);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MedicationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PurchaseCount")
                        .HasColumnType("int");

                    b.HasKey("MedicationId");

                    b.HasIndex("CustomerId");

                    b.ToTable("Medications");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.MedicationItem", b =>
                {
                    b.Property<int>("MedicationItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MedicationItemId"), 1L, 1);

                    b.Property<int>("MedicationId")
                        .HasColumnType("int");

                    b.Property<int>("MedicineId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("MedicationItemId");

                    b.HasIndex("MedicationId");

                    b.HasIndex("MedicineId");

                    b.ToTable("MedicationItems");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Medicine", b =>
                {
                    b.Property<int>("MedicineId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MedicineId"), 1L, 1);

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("CurrentQuantity")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("FeedbackCount")
                        .HasColumnType("float");

                    b.Property<double>("FeedbackSum")
                        .HasColumnType("float");

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("ItemPerPack")
                        .HasColumnType("int");

                    b.Property<string>("MedicineName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("RecentPurchasePrice")
                        .HasColumnType("float");

                    b.Property<double>("SellingPrice")
                        .HasColumnType("float");

                    b.Property<int>("TotalNumberOfPurchase")
                        .HasColumnType("int");

                    b.Property<string>("Weight")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.HasKey("MedicineId");

                    b.HasIndex("BrandId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Medicines");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"), 1L, 1);

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<float>("Discount")
                        .HasColumnType("real");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<double>("PaidAmount")
                        .HasColumnType("float");

                    b.Property<double>("ShipmentCost")
                        .HasColumnType("float");

                    b.Property<double>("TotalAmount")
                        .HasColumnType("float");

                    b.HasKey("OrderId");

                    b.HasIndex("CustomerId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.OrderDetail", b =>
                {
                    b.Property<int>("OrderDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderDetailId"), 1L, 1);

                    b.Property<double>("Cost")
                        .HasColumnType("float");

                    b.Property<bool>("DeliveryStatus")
                        .HasColumnType("bit");

                    b.Property<int>("MedicineId")
                        .HasColumnType("int");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("OrderDetailId");

                    b.HasIndex("MedicineId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Purchase", b =>
                {
                    b.Property<int>("PurchaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PurchaseId"), 1L, 1);

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.Property<double?>("TotalAmount")
                        .HasColumnType("float");

                    b.HasKey("PurchaseId");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.PurchaseDetail", b =>
                {
                    b.Property<int>("PurchaseDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PurchaseDetailId"), 1L, 1);

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("MedicineId")
                        .HasColumnType("int");

                    b.Property<int>("PurchaseId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("TotalSum")
                        .HasColumnType("int");

                    b.Property<int>("VendorId")
                        .HasColumnType("int");

                    b.HasKey("PurchaseDetailId");

                    b.HasIndex("MedicineId");

                    b.HasIndex("PurchaseId");

                    b.HasIndex("VendorId");

                    b.ToTable("PurchaseDetails");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Stock", b =>
                {
                    b.Property<int>("StockId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StockId"), 1L, 1);

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("MedicineId")
                        .HasColumnType("int");

                    b.Property<int>("PurchaseDetailId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("StockId");

                    b.HasIndex("MedicineId");

                    b.HasIndex("PurchaseDetailId");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.UserCredential", b =>
                {
                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccountStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BankBalance")
                        .HasColumnType("int");

                    b.Property<byte[]>("HasedPassword")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Email");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserCredentials");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Vendor", b =>
                {
                    b.Property<int>("VendorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VendorId"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VendorName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VendorId");

                    b.ToTable("Vendors");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Cart", b =>
                {
                    b.HasOne("PharmacyManagementApi.Models.Customer", "Customer")
                        .WithMany("Carts")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PharmacyManagementApi.Models.Medicine", "Medicine")
                        .WithMany()
                        .HasForeignKey("MedicineId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Medicine");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.DeliveryDetail", b =>
                {
                    b.HasOne("PharmacyManagementApi.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PharmacyManagementApi.Models.Medicine", "Medicine")
                        .WithMany()
                        .HasForeignKey("MedicineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PharmacyManagementApi.Models.OrderDetail", "OrderDetail")
                        .WithMany("DeliveryDetails")
                        .HasForeignKey("OrderDetailId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Medicine");

                    b.Navigation("OrderDetail");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Feedback", b =>
                {
                    b.HasOne("PharmacyManagementApi.Models.Customer", "Customer")
                        .WithMany("Feedbacks")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PharmacyManagementApi.Models.Medicine", "Medicine")
                        .WithMany("Feedbacks")
                        .HasForeignKey("MedicineId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Medicine");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Medication", b =>
                {
                    b.HasOne("PharmacyManagementApi.Models.Customer", "Customer")
                        .WithMany("Medications")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.MedicationItem", b =>
                {
                    b.HasOne("PharmacyManagementApi.Models.Medication", "Medication")
                        .WithMany("MedicationItems")
                        .HasForeignKey("MedicationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PharmacyManagementApi.Models.Medicine", "Medicine")
                        .WithMany()
                        .HasForeignKey("MedicineId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Medication");

                    b.Navigation("Medicine");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Medicine", b =>
                {
                    b.HasOne("PharmacyManagementApi.Models.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PharmacyManagementApi.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Order", b =>
                {
                    b.HasOne("PharmacyManagementApi.Models.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.OrderDetail", b =>
                {
                    b.HasOne("PharmacyManagementApi.Models.Medicine", "Medicine")
                        .WithMany()
                        .HasForeignKey("MedicineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PharmacyManagementApi.Models.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Medicine");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.PurchaseDetail", b =>
                {
                    b.HasOne("PharmacyManagementApi.Models.Medicine", "Medicine")
                        .WithMany()
                        .HasForeignKey("MedicineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PharmacyManagementApi.Models.Purchase", "Purchase")
                        .WithMany("PurchaseDetails")
                        .HasForeignKey("PurchaseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PharmacyManagementApi.Models.Vendor", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medicine");

                    b.Navigation("Purchase");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Stock", b =>
                {
                    b.HasOne("PharmacyManagementApi.Models.Medicine", "Medicine")
                        .WithMany()
                        .HasForeignKey("MedicineId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PharmacyManagementApi.Models.PurchaseDetail", "PurchaseDetail")
                        .WithMany()
                        .HasForeignKey("PurchaseDetailId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Medicine");

                    b.Navigation("PurchaseDetail");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.UserCredential", b =>
                {
                    b.HasOne("PharmacyManagementApi.Models.Customer", "Customer")
                        .WithOne()
                        .HasForeignKey("PharmacyManagementApi.Models.UserCredential", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Customer", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("Feedbacks");

                    b.Navigation("Medications");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Medication", b =>
                {
                    b.Navigation("MedicationItems");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Medicine", b =>
                {
                    b.Navigation("Feedbacks");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.OrderDetail", b =>
                {
                    b.Navigation("DeliveryDetails");
                });

            modelBuilder.Entity("PharmacyManagementApi.Models.Purchase", b =>
                {
                    b.Navigation("PurchaseDetails");
                });
#pragma warning restore 612, 618
        }
    }
}

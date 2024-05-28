using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Context
{
    public class PharmacyContext:DbContext
    {
        public PharmacyContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DeliveryDetail> DeliveryDetails { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }  
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Foriegn Key for Cart
            modelBuilder.Entity<Cart>()
             .HasOne(uc => uc.Customer)
             .WithMany(c=>c.Carts)
             .HasForeignKey(c => c.CustomerId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired();
              modelBuilder.Entity<Cart>()
             .HasOne(uc => uc.Medicine)
             .WithMany()
             .HasForeignKey(c => c.MedicineId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired();

            //Foriegn Key for Order
            modelBuilder.Entity<Order>()
            .HasOne(o=>o.Customer)
            .WithMany(c=>c.Orders)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
         

            //Foriegn Key for Feedback
            modelBuilder.Entity<Feedback>()
            .HasOne(f=>f.Customer)
            .WithMany(c=>c.Feedbacks)
            .HasForeignKey(f => f.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.Rating)
                    .HasColumnType("float")
                    .IsRequired();
            });


            modelBuilder.Entity<Feedback>()
            .HasOne(f=>f.Medicine)
            .WithMany(m=>m.Feedbacks)
            .HasForeignKey(f=>f.MedicineId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

            //Foriegn Key for DeliveryDetail
            modelBuilder.Entity<DeliveryDetail>()
            .HasOne(dd => dd.OrderDetail)
            .WithMany(od=>od.DeliveryDetails)
            .HasForeignKey(dd=>dd.OrderDetailId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

             modelBuilder.Entity<DeliveryDetail>()
            .HasOne(dd => dd.Customer)
            .WithMany()
            .HasForeignKey(dd=>dd.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();


            //Foriegn Key for Order Details
            modelBuilder.Entity<OrderDetail>()
            .HasOne(od=>od.Order)
            .WithMany(o=>o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

      

             //Foriegn Key for Medicine
            modelBuilder.Entity<Medicine>()
            .HasOne(m=>m.Category)
            .WithMany()
            .HasForeignKey(m=>m.CategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

            //Foriegn key for purchase
              modelBuilder.Entity<PurchaseDetail>()
             .HasOne(pd=>pd.Purchase)
             .WithMany(p=>p.PurchaseDetails)
             .HasForeignKey(pd=>pd.PurchaseId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired();

        

            //Foriegn key for Stock
            modelBuilder.Entity<Stock>()
           .HasOne(s=>s.Medicine)
           .WithMany()
           .HasForeignKey(s=>s.MedicineId)
           .OnDelete(DeleteBehavior.Restrict)
           .IsRequired();

            modelBuilder.Entity<Stock>()
           .HasOne(s=>s.PurchaseDetail)
           .WithMany()
           .HasForeignKey(s=>s.PurchaseDetailId)
           .OnDelete(DeleteBehavior.Restrict)
           .IsRequired();




            modelBuilder.Entity<Customer>()
            .HasIndex(u => u.Email)
            .IsUnique();

            modelBuilder.Entity<UserCredential>()
                .HasOne(uc => uc.Customer)
                .WithOne()
                .HasForeignKey<UserCredential>(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();



        }



    }
}

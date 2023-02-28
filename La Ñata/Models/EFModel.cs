using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace La_Ñata.Models
{
    public partial class EFModel : DbContext
    {
        public EFModel()
            : base("name=AzureDB")
        {
        }

        public virtual DbSet<Expense> Expense { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductOrder> ProductOrder { get; set; }
        public virtual DbSet<Rol> Rol { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.client_name)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.phone)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.event_adress)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.observation)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.ProductOrder)
                .WithRequired(e => e.Order)
                .HasForeignKey(e => e.id_order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductOrder)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.id_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Rol>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Rol>()
                .HasMany(e => e.User)
                .WithRequired(e => e.Rol)
                .HasForeignKey(e => e.id_rol)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.password)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}

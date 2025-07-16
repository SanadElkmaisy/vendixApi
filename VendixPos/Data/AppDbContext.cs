using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using VendixPos.DTOs;
using VendixPos.Models;

namespace VendixPos.Data
{
    public class AppDbContext : DbContext
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Item> ItemsTable { get; set; }
        public DbSet<Barcode> Barcode { get; set; }
        public DbSet<FastITemsWebPos> FastITemsWebPos { get; set; }
        public DbSet<FastGroupWebPos> FastGroupWebPos { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Units> Units { get; set; }


        public DbSet<Item> Items { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Frozen> Frozen { get; set; }
        public DbSet<SalesTransactionResult> SalesTransactionResults { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
    
            modelBuilder.Entity<Item>().ToTable("ItemsTable");
            modelBuilder.Entity<Barcode>().ToTable("Barcode").ToTable("BarcodeTable");
            modelBuilder.Entity<FastITemsWebPos>().ToTable("FastITemsWebPos");
            modelBuilder.Entity<FastGroupWebPos>().ToTable("FastGroupWebPos");
            modelBuilder.Entity<Invoice>().ToTable("Invoice").ToTable("InvoiceTable"); 
            modelBuilder.Entity<Inventory>().ToTable("Inventory").ToTable("InventoryTable"); 
            modelBuilder.Entity<Units>().ToTable("Units").ToTable("UnitsTable");
            modelBuilder.Entity<Supplier>().ToTable("Suplier");
            modelBuilder.Entity<Frozen>().ToTable("FrozenTable");
            modelBuilder.Entity<SalesTransactionResult>().HasNoKey().ToView(null);

        }

    }
}

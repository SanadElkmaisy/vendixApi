using Microsoft.EntityFrameworkCore;
using VendixPos.DTOs;
using VendixPos.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace VendixPos.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Your existing DbSets
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
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<AddInventory> AddInventory { get; set; }

        // User-related DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UsersPermission> UsersPermissions { get; set; }
        public DbSet<GroupPermission> GroupPermissions { get; set; }
        public DbSet<GroupPermissionSetting> GroupPermissionSettings { get; set; }
        public DbSet<SalesTransactionHistoryDto> SalesTransactionHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Your existing configurations
            modelBuilder.Entity<Item>().ToTable("ItemsTable");
            modelBuilder.Entity<Barcode>().ToTable("BarcodeTable");
            modelBuilder.Entity<FastITemsWebPos>().ToTable("FastITemsWebPos");
            modelBuilder.Entity<FastGroupWebPos>().ToTable("FastGroupWebPos");
            modelBuilder.Entity<Invoice>().ToTable("InvoiceTable");
            modelBuilder.Entity<Inventory>().ToTable("InventoryTable");
            modelBuilder.Entity<Units>().ToTable("UnitsTable");
            modelBuilder.Entity<Supplier>().ToTable("Suplier");
            modelBuilder.Entity<Frozen>().ToTable("FrozenTable");
            modelBuilder.Entity<SalesTransactionResult>().HasNoKey().ToView(null);

            // PaymentMethod configuration
            modelBuilder.Entity<PaymentMethod>()
                .ToTable("PaymentMothedTable")
                .HasKey(p => p.PaymentMethodId);

            modelBuilder.Entity<PaymentMethod>()
                .Property(p => p.PaymentMethodId)
                .HasColumnName("PaymentMothetId");

            modelBuilder.Entity<PaymentMethod>()
                .Property(p => p.PaymentmothetName)
                .HasColumnName("PaymentmothetName")
                .IsRequired();

            // ===== USER-RELATED CONFIGURATIONS =====

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Username).IsUnique();

                // Column mappings
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FullName)
                    .HasMaxLength(100);

                entity.Property(e => e.AuthenticationCode)
                    .HasColumnName("AttenticationCode")
                    .HasMaxLength(50);

                entity.Property(e => e.AuthenticationState)
                    .HasColumnName("AttenticationState");

                entity.Property(e => e.Image)
                    .HasColumnName("image");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

           
                // Foreign key to Group
                entity.HasOne(u => u.Group)
                    .WithMany(g => g.Users)
                    .HasForeignKey(u => u.GroupId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Group configuration
            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("Groups");
                entity.HasKey(e => e.GroupId);
                entity.HasIndex(e => e.GroupName).IsUnique();

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(200);
            });

            // UsersPermission configuration
            modelBuilder.Entity<UsersPermission>(entity =>
            {
                entity.ToTable("UsersPermissions");
                entity.HasKey(e => e.PermissionId);
                entity.HasIndex(e => e.PermissionName).IsUnique();

                entity.Property(e => e.PermissionName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(200);
            });

            // GroupPermission configuration (composite key table)
            modelBuilder.Entity<GroupPermission>(entity =>
            {
                entity.ToTable("GroupPermissions");
                entity.HasKey(e => new { e.GroupId, e.PermissionId });

                // Foreign key to Group
                entity.HasOne(gp => gp.Group)
                    .WithMany(g => g.GroupPermissions)
                    .HasForeignKey(gp => gp.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Foreign key to UsersPermission
                entity.HasOne(gp => gp.UsersPermission)
                    .WithMany(up => up.GroupPermissions)
                    .HasForeignKey(gp => gp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // GroupPermissionSetting configuration (composite key table)
            modelBuilder.Entity<GroupPermissionSetting>(entity =>
            {
                entity.ToTable("GroupPermissionSettings");
                entity.HasKey(e => new { e.GroupId, e.PermissionId });

                entity.Property(e => e.SettingValue)
                    .HasMaxLength(50);

                // Foreign key to GroupPermission (using composite foreign key)
                entity.HasOne(gps => gps.GroupPermission)
                    .WithMany(gp => gp.GroupPermissionSettings)
                    .HasForeignKey(gps => new { gps.GroupId, gps.PermissionId })
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<SalesTransactionHistoryDto>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null); // Not mapped to a table
            });
        }
    }
}

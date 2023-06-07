using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace dotnet_products_rest_api.Models;

public partial class StoreDbContext : DbContext
{
    public StoreDbContext()
    {
    }

    public StoreDbContext(DbContextOptions<StoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Merchant> Merchants { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3307;database=store_db;user=root;password=password", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.3-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PRIMARY");

            entity.ToTable("countries");

            entity.Property(e => e.Code)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("code");
            entity.Property(e => e.ContinentName)
                .HasMaxLength(100)
                .HasColumnName("continent_name");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("state");
        });

        modelBuilder.Entity<Merchant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("merchants");

            entity.HasIndex(e => e.AdminId, "merchants_FK");

            entity.HasIndex(e => e.CountryCode, "merchants_FK_1");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.AdminId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("admin_id");
            entity.Property(e => e.CountryCode)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("country_code");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.MerchantName)
                .HasMaxLength(150)
                .HasColumnName("merchant_name");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("state");

            entity.HasOne(d => d.Admin).WithMany(p => p.Merchants)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("merchants_FK");

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.Merchants)
                .HasForeignKey(d => d.CountryCode)
                .HasConstraintName("merchants_FK_1");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.UserId, "orders_FK");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("state");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.UserId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("orders_FK");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("order_items");

            entity.HasIndex(e => e.OrderId, "order_items_FK");

            entity.HasIndex(e => e.ProductId, "order_items_FK_1");

            entity.Property(e => e.OrderId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("order_id");
            entity.Property(e => e.ProductId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(10) unsigned")
                .HasColumnName("quantity");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("state");

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_FK");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_FK_1");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.MerchantId, "products_FK");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.MerchantId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("merchant_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("state");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");

            entity.HasOne(d => d.Merchant).WithMany(p => p.Products)
                .HasForeignKey(d => d.MerchantId)
                .HasConstraintName("products_FK");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.CountryCode, "users_FK");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CountryCode)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("country_code");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("state");

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.CountryCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_FK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

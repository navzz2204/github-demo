using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ToyStoreOnlineWeb.Models;

public partial class ToyStoreDbContext : DbContext
{
    public ToyStoreDbContext()
    {
    }

    public ToyStoreDbContext(DbContextOptions<ToyStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessTimesCount> AccessTimesCounts { get; set; }

    public virtual DbSet<Age> Ages { get; set; }

    public virtual DbSet<Decentralization> Decentralizations { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<ItemCart> ItemCarts { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Producer> Producers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductCategoryParent> ProductCategoryParents { get; set; }

    public virtual DbSet<ProductViewed> ProductVieweds { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    public virtual DbSet<UsersSpin> UsersSpins { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("Server=MSI;Database=ToyStoreDB;Trusted_Connection=True;TrustServerCertificate=True;")
            .UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessTimesCount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.AccessTimesCount");

            entity.ToTable("AccessTimesCount");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
        });

        modelBuilder.Entity<Age>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Ages");

            entity.ToTable("Age");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.Seokeyword)
                .HasMaxLength(256)
                .HasColumnName("SEOKeyword");
        });

        modelBuilder.Entity<Decentralization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Decentralization");

            entity.ToTable("Decentralization");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserTypeId).HasColumnName("UserTypeID");

            entity.HasOne(d => d.Role).WithMany(p => p.Decentralizations)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Decentralization_Role");

            entity.HasOne(d => d.UserType).WithMany(p => p.Decentralizations)
                .HasForeignKey(d => d.UserTypeId)
                .HasConstraintName("FK_Decentralization_UserType");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Gender");

            entity.ToTable("Gender");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<ItemCart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Cart");

            entity.ToTable("ItemCart");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.ItemCarts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ItemCart_Products");

            entity.HasOne(d => d.User).WithMany(p => p.ItemCarts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ItemCart_User");
        });


        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Message");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FromUserId).HasColumnName("FromUserID");
            entity.Property(e => e.ToUserId).HasColumnName("ToUserID");

            entity.HasOne(d => d.FromUser).WithMany(p => p.MessageFromUsers)
                .HasForeignKey(d => d.FromUserId)
                .HasConstraintName("FK_Message_User");

            entity.HasOne(d => d.ToUser).WithMany(p => p.MessageToUsers)
                .HasForeignKey(d => d.ToUserId)
                .HasConstraintName("FK_Message_User1");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Order");

            entity.ToTable("Order");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateOrder).HasColumnType("datetime");
            entity.Property(e => e.DateShip).HasColumnType("datetime");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.OrderDetail");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetail_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_OrderDetail_Products");
        });

        modelBuilder.Entity<Producer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Producer");

            entity.ToTable("Producer");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.Seokeyword)
                .HasMaxLength(256)
                .HasColumnName("SEOKeyword");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Products");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AgeId).HasColumnName("AgeID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.GenderId).HasColumnName("GenderID");
            entity.Property(e => e.Image1).HasMaxLength(256);
            entity.Property(e => e.Image2).HasMaxLength(256);
            entity.Property(e => e.Image3).HasMaxLength(256);
            entity.Property(e => e.Image4).HasMaxLength(256);
            entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProducerId).HasColumnName("ProducerID");
            entity.Property(e => e.PromotionPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Seokeyword)
                .HasMaxLength(256)
                .HasColumnName("SEOKeyword");

            entity.HasOne(d => d.Age).WithMany(p => p.Products)
                .HasForeignKey(d => d.AgeId)
                .HasConstraintName("FK_Products_Age");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Products_ProductCategory");

            entity.HasOne(d => d.Gender).WithMany(p => p.Products)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_Products_Gender");

            entity.HasOne(d => d.Producer).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProducerId)
                .HasConstraintName("FK_Products_Producer");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.ProductCategory");

            entity.ToTable("ProductCategory");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.Seokeyword)
                .HasMaxLength(256)
                .HasColumnName("SEOKeyword");

            entity.HasOne(d => d.Parent).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_ProductCategory_ProductCategoryParent");
        });

        modelBuilder.Entity<ProductCategoryParent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.ProductCategoryParent");

            entity.ToTable("ProductCategoryParent");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.SeoKeyword).HasMaxLength(256);
        });

        modelBuilder.Entity<ProductViewed>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.ProductViewed");

            entity.ToTable("ProductViewed");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVieweds)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductViewed_Products");

            entity.HasOne(d => d.User).WithMany(p => p.ProductVieweds)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ProductViewed_User");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Rating");

            entity.ToTable("Rating");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Rating_Products");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Rating_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Role");

            entity.ToTable("Role");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NameDisplay).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.User");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AmountPurchased).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UserTypeId).HasColumnName("UserTypeID");

            entity.HasOne(d => d.UserType).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserTypeId)
                .HasConstraintName("FK_User_UserType");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.UserType");

            entity.ToTable("UserType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(256);
        });
        modelBuilder.Entity<ItemCart>().Ignore(c => c.Name);
        modelBuilder.Entity<ItemCart>().Ignore(c => c.Image);
        modelBuilder.Entity<ItemCart>().Ignore(c => c.Price);
        modelBuilder.Entity<UsersSpin>(entity =>
        {
            entity.ToTable("UsersSpin");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UsersSpins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UsersSpin_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

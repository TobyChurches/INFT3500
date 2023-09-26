using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebStore.Models;

public partial class StoreDbContext : DbContext
{
    public StoreDbContext()
    {
    }

    public StoreDbContext(DbContextOptions<StoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookGenre> BookGenres { get; set; }

    public virtual DbSet<GameGenre> GameGenres { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<MovieGenre> MovieGenres { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductsInOrder> ProductsInOrders { get; set; }

    public virtual DbSet<Source> Sources { get; set; }

    public virtual DbSet<Stocktake> Stocktakes { get; set; }

    public virtual DbSet<To> Tos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:StoreDbConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookGenre>(entity =>
        {
            entity.HasKey(e => e.SubGenreId);

            entity.ToTable("Book_genre");

            entity.Property(e => e.SubGenreId).HasColumnName("subGenreID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<GameGenre>(entity =>
        {
            entity.HasKey(e => e.SubGenreId);

            entity.ToTable("Game_genre");

            entity.Property(e => e.SubGenreId).HasColumnName("subGenreID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("Genre");

            entity.Property(e => e.GenreId)
                .ValueGeneratedNever()
                .HasColumnName("genreID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<MovieGenre>(entity =>
        {
            entity.HasKey(e => e.SubGenreId);

            entity.ToTable("Movie_genre");

            entity.Property(e => e.SubGenreId).HasColumnName("subGenreID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Customer).HasColumnName("customer");
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.StreetAddress).HasMaxLength(255);
            entity.Property(e => e.Suburb).HasMaxLength(255);

            entity.HasOne(d => d.CustomerNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Customer)
                .HasConstraintName("FK_Orders_TO");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Author).HasMaxLength(255);
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Published).HasColumnType("date");
            entity.Property(e => e.SubGenre).HasColumnName("subGenre");

            entity.HasOne(d => d.GenreNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Genre)
                .HasConstraintName("FK_Product_Genre");

            entity.HasOne(d => d.LastUpdatedByNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.LastUpdatedBy)
                .HasConstraintName("FK_Product_Users");
        });

        modelBuilder.Entity<ProductsInOrder>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.ProduktId).HasColumnName("produktId");

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_ProductsInOrders_Orders");

            entity.HasOne(d => d.Produkt).WithMany()
                .HasForeignKey(d => d.ProduktId)
                .HasConstraintName("FK_ProductsInOrders_Stocktake");
        });

        modelBuilder.Entity<Source>(entity =>
        {
            entity.ToTable("Source");

            entity.Property(e => e.Sourceid).HasColumnName("sourceid");
            entity.Property(e => e.ExternalLink).HasColumnName("externalLink");
            entity.Property(e => e.SourceName).HasColumnName("Source_name");

            entity.HasOne(d => d.GenreNavigation).WithMany(p => p.Sources)
                .HasForeignKey(d => d.Genre)
                .HasConstraintName("FK_Source_Genre");
        });

        modelBuilder.Entity<Stocktake>(entity =>
        {
            entity.HasKey(e => e.ItemId);

            entity.ToTable("Stocktake");

            entity.HasOne(d => d.Product).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Stocktake_Product");

            entity.HasOne(d => d.Source).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("FK_Stocktake_Source");
        });

        modelBuilder.Entity<To>(entity =>
        {
            entity.HasKey(e => e.CustomerId);

            entity.ToTable("TO");

            entity.Property(e => e.CustomerId).HasColumnName("customerID");
            entity.Property(e => e.CardNumber).HasMaxLength(50);
            entity.Property(e => e.CardOwner).HasMaxLength(50);
            entity.Property(e => e.Cvv).HasColumnName("CVV");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Expiry)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.StreetAddress).HasMaxLength(255);
            entity.Property(e => e.Suburb).HasMaxLength(50);

            entity.HasOne(d => d.Patron).WithMany(p => p.Tos)
                .HasPrincipalKey(p => p.UserId)
                .HasForeignKey(d => d.PatronId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TO_Patrons");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserName).HasName("PK_Users");

            entity.ToTable("User");

            entity.HasIndex(e => e.UserId, "UQ_UserID").IsUnique();

            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.HashPw)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("HashPW");
            entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");
            entity.Property(e => e.IsEmployee).HasColumnName("isEmployee");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Salt)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("UserID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

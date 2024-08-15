using System;
using AmazonSalesAnalysis.Context;
using AmazonSalesAnalysis.Model;
using Microsoft.EntityFrameworkCore;

namespace Entity_Framework.Context
{
	public class DataContext : DbContext, IDataContext
    {
		public DataContext(DbContextOptions<DataContext> options): base(options)
		{

		}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=AmazonDataset;User=sa;Password=Samira#27;TrustServerCertificate=True", options => options.EnableRetryOnFailure());
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<CategoryAnalysis> categoryAnalysis { get; set; }
        public DbSet<SubCategoryAnalysis> subCategoryAnalysis { get; set; }
        public DbSet<ProductAnalysis> ProductAnalysis { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoryAnalysis>()
                .HasNoKey();
            modelBuilder.Entity<SubCategoryAnalysis>()
                .HasNoKey();
            modelBuilder.Entity<ProductAnalysis>()
                .HasNoKey();

            modelBuilder.Entity<CategoryAnalysis>()
                .Property(a => a.AverageRating)
                .HasColumnName("Average_Rating");

            modelBuilder.Entity<CategoryAnalysis>()
                .Property(a => a.TotalSold)
                .HasColumnName("Total_Sold");

            modelBuilder.Entity<SubCategoryAnalysis>()
                .Property(a => a.AverageRating)
                .HasColumnName("Average_Rating");

            modelBuilder.Entity<SubCategoryAnalysis>()
                .Property(a => a.TotalSold)
                .HasColumnName("Total_Sold");


            modelBuilder.Entity<Invoice>()
                .ToTable("Invoices");

            modelBuilder.Entity<Category>()
                .Property(a=> a.CategoryName)
                .HasColumnName("Category");

            modelBuilder.Entity<Rating>()
                .Property(a => a.RatingScore)
                .HasColumnName("Rating");
        }
    }
}


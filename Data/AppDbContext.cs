using EMS_Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMS_Backend.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<FunctionMaster> FunctionMasters => Set<FunctionMaster>();
        public DbSet<RoleFunctions> RoleFunctions => Set<RoleFunctions>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Product> Product => Set<Product>();
        public DbSet<ProductImage> ProductImage => Set<ProductImage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure
            ConfigurateRoles(modelBuilder);
            ConfigurateUsers(modelBuilder);
            ConfigurateCategories(modelBuilder);
            ConfigurateProducts(modelBuilder);
            ConfigurateSuppliers(modelBuilder);
            ConfigurateFunctionMasters(modelBuilder);
            ConfigurateRoleFunctions(modelBuilder);
        }

        #region Roles
        private void ConfigurateRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(100);
            });
        }
        #endregion

        #region FunctionMaster
        private void ConfigurateFunctionMasters(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FunctionMaster>(entity =>
            {
                entity.HasKey(e => e.FunctionId);
                entity.Property(e => e.FunctionName).IsRequired().HasMaxLength(100);
            });
        }
        #endregion

        #region RoleFunctions
        private void ConfigurateRoleFunctions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleFunctions>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.FunctionId });
                entity.Property(e => e.IsActive).IsRequired();
                // Relationships
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.RoleFunctions)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.FunctionMaster)
                      .WithMany(f => f.RoleFunctions)
                      .HasForeignKey(e => e.FunctionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
        #endregion

        #region Users
        private void ConfigurateUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.RoleId).IsRequired();
                // Relationship
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
        #endregion

        #region Categories
        private void ConfigurateCategories(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ParentCategoryId).IsRequired(false);
            });
        }
        #endregion

        #region Products
        private void ConfigurateProducts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.BasePrice).IsRequired();
                entity.Property(e => e.ThumbnailUrl).IsRequired();
                entity.Property(e => e.IsVariant).HasDefaultValue(false);
                entity.Property(e => e.CategoryId).IsRequired();
                entity.Property(e => e.SupplierId).IsRequired();
                // Relationship
                entity.HasOne(e => e.Category)
                      .WithMany(r => r.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Supplier)
                        .WithMany(r => r.Products)
                        .HasForeignKey(e => e.SupplierId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
        }
        #endregion

        #region ProductImages
        private void ConfigurateProductImages (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageProductElement);
                entity.Property(e => e.ProductId).IsRequired();
                // Relationship
                entity.HasOne(e => e.Product)
                      .WithMany(r => r.ProductImages)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
        #endregion

        #region Suppliers
        private void ConfigurateSuppliers(ModelBuilder modelBuilder)
        {
            var e = modelBuilder.Entity<Supplier>();
            e.HasKey(e => e.Id);
            e.Property(su => su.Id).IsRequired().HasMaxLength(20);
            e.Property(su => su.SupplierName).IsRequired().HasMaxLength(100);
            e.Property(su => su.SupplierAddress).IsRequired().HasMaxLength(100);
            e.Property(su => su.SupplierPhone).IsRequired().HasMaxLength(10);
        }
        #endregion
    }
}

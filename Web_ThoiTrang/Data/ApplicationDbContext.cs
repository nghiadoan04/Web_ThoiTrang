
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using Web_ThoiTrang.Models; // Đảm bảo import Models namespace

namespace Web_ThoiTrang.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Định nghĩa DbSet cho các Model của bạn
        public DbSet<User> Users { get; set; }
        // Thêm các DbSet khác cho Product, Order, Cart, v.v. khi bạn phát triển các chức năng đó
        // public DbSet<Product> Products { get; set; }
        // public DbSet<Order> Orders { get; set; }
        // public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình thêm cho Model nếu cần (ví dụ: khóa chính composite, mối quan hệ)
            // Ví dụ: Đảm bảo Username là duy nhất
            // modelBuilder.Entity<User>()
            //     .HasIndex(u => u.Username)
            //     .IsUnique();
        }
    }
}
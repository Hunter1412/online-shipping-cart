using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Data;
public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Product>? Products { get; set; }
    public DbSet<Image>? Images { get; set; }
    public DbSet<Inventory>? Inventories { get; set; }
    public DbSet<Contact>? Contacts { get; set; }
    public DbSet<Feedback>? Feedbacks { get; set; }
    public DbSet<Cart>? Carts { get; set; }
    public DbSet<Voucher>? Vouchers { get; set; }
    public DbSet<Shipping>? Shippings { get; set; }
    public DbSet<Order>? Orders { get; set; }
    public DbSet<OrderDetail>? OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //cat chuoi aspnet truoc ten cua table
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tbName = entityType.GetTableName();
            if (tbName!.StartsWith("AspNet"))
            {
                entityType.SetTableName(tbName.Substring(6));
            }
        }
        //many - many
        builder.Entity<Product>()
        .HasMany(e => e.Orders)
        .WithMany(e => e.Products)
        .UsingEntity<OrderDetail>(
            l => l.HasOne<Order>(e => e.Order).WithMany(e => e.OrderDetails).HasForeignKey(e => e.OrderId),
            r => r.HasOne<Product>(e => e.Product).WithMany(e => e.OrderDetails).HasForeignKey(e => e.ProductId));
        //one - one
        builder.Entity<Shipping>()
            .HasOne(e => e.Order)
            .WithOne(e => e.Shipping);

        builder.Entity<OrderDetail>()
                .HasIndex(u => u.OrderNumber)
                .IsUnique();

        builder.Entity<Product>()
                .HasIndex(p => p.Slug)
                .IsUnique();

        builder.Entity<Category>()
                .HasIndex(p => p.Slug)
                .IsUnique();

        builder.Entity<Voucher>()
                .HasIndex(p => p.Code)
                .IsUnique();
    }
}

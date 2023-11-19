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
    public DbSet<Contact>? Contacts { get; set; }
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Product>? Products { get; set; }
    public DbSet<Cart>? Carts { get; set; }
    public DbSet<Feedback>? Feedbacks { get; set; }
    public DbSet<Image>? Images { get; set; }
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
    }
}

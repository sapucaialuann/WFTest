using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WFTest.Models
{
    public class ProductContext : DbContext
    {
        public ProductContext() : base("WFTest")
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> ShoppingCartItems { get; set; }
        public DbSet<DBOrder> Order { get; set; }
    }
}
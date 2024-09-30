using System.Collections.Generic;
using System.Data.Entity;

namespace WFTest.Models
{
    public class ProductDatabaseInitializer : DropCreateDatabaseAlways<ProductContext>
    {
        protected override void Seed(ProductContext context)
        {
            context.Categories.AddRange(GetCategories());
            context.Products.AddRange(GetProducts());

            context.SaveChanges();
        }
        private static List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Horses"
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Cars"
                }
            };
        }
        private static List<Product> GetProducts() 
        {
            return new List<Product>
            {
                new Product
                {
                    ProductId = 1,
                    ProductName = "Brown Horse",
                    ProductDescription = "Product1 Description test",
                    ProductPrice = 10.49,
                    ImagePath = "horse.jpg",
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "White Horse",
                    ProductDescription = "Product2 Description test",
                    ProductPrice = 8.49,
                    ImagePath = "white-horse.jpg",
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "Mercedes Car",
                    ProductDescription = "Product3 Description test",
                    ProductPrice = 1000.49,
                    ImagePath = "old-merc.jpg",
                    CategoryId = 2
                }
            };
        }
    }
}
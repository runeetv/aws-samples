using System;
using aurora_postgresql_dotnet.Models;
using Microsoft.EntityFrameworkCore;

namespace aurora_postgresql_dotnet
{
    class Program
    {   
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to exit");
            //InsertProductsAndCategories();
            //GetListOfProductsWithCategoryNames();
            //SearchProductsByName("Phone");
            //UpdateProductPrice();
            DeleteProduct();

            Console.ReadLine();
        }

        static void InsertProductsAndCategories()
        {
            GadgetsOnlineContext context = new GadgetsOnlineContext();
            if (context.Categories.Count() == 0)
            {
                IList<Category> categories = new List<Category>
                {
                    new Category { Name = "Mobile Phones", Description = "Latest collection of Mobile Phones" } ,
                    new Category { Name = "Laptops", Description = "Latest Laptops in 2022" } ,
                    new Category { Name = "Desktops", Description = "Latest Desktops in 2022" } ,
                    new Category { Name = "Audio", Description = "Latest audio devices" } ,
                    new Category { Name = "Accessories", Description = "USB Cables, Mobile chargers and Keyboards etc" } ,
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            if (context.Products.Count() == 0)
            {
                IList<Product> products = new List<Product>
                {
                    new Product{CategoryId=1,Name="Phone 12",Price=699.00M,ProductArtUrl="/Content/Images/Mobile/1.jpg"},
                    new Product{CategoryId=1,Name="Phone 13 Pro",Price=999.00M,ProductArtUrl="/Content/Images/Mobile/2.jpg"},
                    new Product{CategoryId=1,Name="Phone 13 Pro Max",Price=1199.00M,ProductArtUrl="/Content/Images/Mobile/3.jpg"},
                    new Product{CategoryId=2,Name="XTS 13'",Price=899.00M,ProductArtUrl="/Content/Images/Laptop/1.jpg"},
                    new Product{CategoryId=2,Name="PC 15.5'",Price=479.00M,ProductArtUrl="/Content/Images/Laptop/2.jpg"},
                    new Product{CategoryId=2,Name="Notebook 14",Price=169.00M,ProductArtUrl="/Content/Images/Laptop/3.jpg"},
                    new Product{CategoryId=3,Name="The IdeaCenter",Price=539.00M,ProductArtUrl="/Content/Images/placeholder.gif"},
                    new Product{CategoryId=3,Name="COMP 22-df003w",Price=389.00M,ProductArtUrl="/Content/Images/placeholder.gif"},
                    new Product{CategoryId=4,Name="Bluetooth Headphones Over Ear",Price=28.00M,ProductArtUrl="/Content/Images/Headphones/1.png"},
                    new Product{CategoryId=4,Name="ZX Series ",Price=10.00M,ProductArtUrl="/Content/Images/Headphones/2.png"},
                    new Product{CategoryId=5,Name="Wireless charger",Price=9.99M,ProductArtUrl="/Content/Images/placeholder.gif"},
                    new Product{CategoryId=5,Name="Mousepad",Price=2.99M,ProductArtUrl="/Content/Images/placeholder.gif"},
                    new Product{CategoryId=5,Name="Keyboard",Price=9.99M,ProductArtUrl="/Content/Images/placeholder.gif"},

                };
                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }

        static void GetListOfProductsWithCategoryNames()
        {
            GadgetsOnlineContext context = new GadgetsOnlineContext();

            var products = context.Products.ToList();            

            foreach (var product  in products)
            {
                Console.WriteLine("Product : {0} , Category : {1}", product.Name, product.Category.Name);
            }

        }

        static void SearchProductsByName(string prodName)
        {
            GadgetsOnlineContext context = new GadgetsOnlineContext();

            var products = context.Products.ToList().Where( p => p.Name.Contains(prodName)).ToList();

            foreach (var product in products)
            {
                Console.WriteLine("Product : {0} , Category : {1}", product.Name, product.Category.Name);
            }

        }

        static void UpdateProductPrice()
        {
            GadgetsOnlineContext context = new GadgetsOnlineContext();

            //get produt with ProductId = 13
            var product = context.Products.ToList().Where(p => p.ProductId.Equals(13)).FirstOrDefault();
            var existingPrice = product.Price;
            Console.WriteLine("Product : {0} , has price {1} ", product.Name, existingPrice);
            
            

            //update price of an existing product and save changes
            product.Price = existingPrice + 1;
            context.Update<Product>(product);
            context.SaveChanges();

            //Get updated price from db
            
            product = context.Products.ToList().Where(p => p.ProductId.Equals(13)).FirstOrDefault();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Product : {0} , has new price {1} ", product.Name, product.Price);
            Console.ForegroundColor = ConsoleColor.White;

        }

        static void DeleteProduct()
        {
            GadgetsOnlineContext context = new GadgetsOnlineContext();
            
            //insert product
            var product = new Product { CategoryId = 5, Name = "Dummy Product", Price = 9.99M, ProductArtUrl = "/Content/Images/placeholder.gif" };
            context.Add<Product>(product);
            context.SaveChanges();


            //get produt with Productname 
            product = context.Products.ToList().Where(p => p.Name.Equals("Dummy Product")).FirstOrDefault();            
            Console.WriteLine("Deleting product : {0} , with id {1} ", product.Name, product.ProductId);
            context.Remove<Product>(product);
            context.SaveChanges();


        }


    }
}


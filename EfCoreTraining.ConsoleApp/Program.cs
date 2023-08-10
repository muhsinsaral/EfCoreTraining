using EfCoreTraining.ConsoleApp.DAL;
using EfCoreTraining.ConsoleApp;
using Microsoft.EntityFrameworkCore;

Initializer.Build();

using (var _context = new AppDbContext())
{
    var newCategory = new Category
    {
        Name = "Category 1",
        Products = new List<Product>
        {
            new Product
            {
                Name = "Product 1",
                Price = 100
            },
            new Product
            {
                Name = "Product 2",
                Price = 200
            }
        }
    };

    _context.Categories.Add(newCategory);
    _context.SaveChanges();
}
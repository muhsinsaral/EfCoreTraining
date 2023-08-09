using EfCoreTraining.ConsoleApp.DAL;
using EfCoreTraining.ConsoleApp;
using Microsoft.EntityFrameworkCore;

Initializer.Build();

using (var _context = new AppDbContext())
{
    var newProduct = new Product
    {
        Name = "New Product",
        Price = 100
    };
    Console.WriteLine($"newProduct state: {_context.Entry(newProduct).State}"); // state: Detached

    await _context.AddAsync(newProduct);
    Console.WriteLine("Product is added");
    Console.WriteLine($"newProduct state: {_context.Entry(newProduct).State}"); // state: Added

    await _context.SaveChangesAsync();
    Console.WriteLine("Product is saved");
    Console.WriteLine($"newProduct state: {_context.Entry(newProduct).State}"); // state: Unchanged

    Console.WriteLine("-----------------------------------------------");

    newProduct.Price = 200;
    Console.WriteLine("Product is updated");
    Console.WriteLine($"newProduct state: {_context.Entry(newProduct).State}"); // state: Modified

    await _context.SaveChangesAsync();
    Console.WriteLine("Product is saved");
    Console.WriteLine($"newProduct state: {_context.Entry(newProduct).State}"); // state: Unchanged

    Console.WriteLine("-----------------------------------------------");

    _context.Remove(newProduct);
    Console.WriteLine("Product is removed");
    Console.WriteLine($"newProduct state: {_context.Entry(newProduct).State}"); // state: Deleted

    await _context.SaveChangesAsync();
    Console.WriteLine("Product is saved");
    Console.WriteLine($"newProduct state: {_context.Entry(newProduct).State}"); // state: Detached

    Console.WriteLine("*************************************************");





    var products = await _context.Products.ToListAsync();

    products.ForEach(p =>
    {
        var state = _context.Entry(p).State;
        Console.WriteLine($"{p.Id} - {p.Name} - {p.Price} - state: {state}"); // state: Unchanged
    });

}
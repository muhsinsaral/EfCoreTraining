# EFCore Best Practices
## DbContext 
---
**Add/AddAsync**
**Update**
**Remove**
**Find/FindAsync**
**SaveChanges/SaveChangesAsync**
---
> When **'Add-Update-Remove'** operations are performed on the context, it is not immediately reflected on the database.
> In Memory, its state changes as Added, Modified, Deleted
> It is reflected to the database with the SaveChanges or SaveChangesAsync method.
---
> Note: All changes are kept in memory until SaveChanges is made.
> If there is an error in one of the changes in the memory when SaveChanges is made, all changes are undone.
---
## Entry States
**Unchanged** - It is the state when we first pull the data. `var products = await _context.Products.ToListAsync();`

**Added** - It is the state when we added data.
- `var product = new Product { Name = "Product 1" }	;` Here it is in the 'detached' state.
- `await _context.Products.AddAsync(product);` Here it is in the 'added' state.
- `await _context.SaveChangesAsync();` Here it is in the 'unchanged' state.

**Detached** - It is when we create the data but have not added it to the context yet.
- `var product = new Product { Name = "Product 1" }	;`  Here it is in the 'detached' state.  
- `await _context.SaveChangesAsync();` Here it is in the 'unchanged' state. 

**Deleted** - It is the state when we deleted data.
- `var product = await _context.Products.FindAsync(1);` Here it is in the 'unchanged' state. 
- `await _context.Products.RemoveAsync(product);` Here it is in the 'deleted' state. 
- `await _context.SaveChangesAsync();` Here it is in the 'unchanged' state. 

**Modified** -  It is the state when we updated data.
- `var product = await _context.Products.FindAsync(1);` Here it is in the 'unchanged' state. 
- `product.Name = "Kalem";` Here it is in the 'modified' state. 
- `await _context.SaveChangesAsync();` Here it is in the 'unchanged' state. 
---
## ChangeTracker
**ChangeTracker** - It keeps tracked entities on the context.
- `var changeTracker = _context.ChangeTracker.Entries();` Here it follows the changes on the context.
- `var changeTracker = _context.ChangeTracker.Entries<Product>();` Here it follows the changes in the Product Entity on the context.
- `var changeTracker = _context.ChangeTracker.Entries<Product>().Where(x => x.State == EntityState.Modified).ToList();` Here it lists the changes of the Product Entity on the context and returns the modified status.
---
## DbSet
**Add/AddAsync**
**Update**
**Remove**
**AsNoTracking**
**Find/FindAsync**
**First/FirstOrDefault**
**Single/SingleOrDefault**
**Where**
---

## Relationships
#### One-to-Many

| CategoryId | Name |
| ------ | ------ |
| 1 | Furniture |
| 2 | Technology |

| ProductId | Name |`CategoryId`|
| ------ | ------ |------|
| 1 | Furniture |**1**|
| 2 | Technology |**2**|

``` cs
public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
```
```cs
public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product> Products { get; set; } = new List<Product>(); // if we want to add product via category
    }
```



---
#### One-to-One
> In the relationship we have established below, `Product` is defined as parent and `ProductFeatureId` is defined as child.
> In a One-to-One relationship, the entity marked with the `foreign key` must be the child entity.

| ProductId | Name |
| ------ | ------ |
| 1 | Furniture |
| 2 | Technology |

| `ProductId` | Width | Height | 
| ------ | ------ |------|
| 1 | 100 | 200 |
| 2 | 200 | 50 |

``` cs
public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; } 
        public Category Category { get; set; }
        public ProductFeature ProductFeatures { get; set; }
    }
```

```cs
public class ProductFeature
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Product Product { get; set; }
    }
```

```cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                    .HasOne(p => p.ProductFeatures)
                    .WithOne(p => p.Product)
                    .HasForeignKey<ProductFeature>(p => p.Id);
            
            base.OnModelCreating(modelBuilder);
        }
```

---
#### Many-to-Many

| StudentId | Name |
| ------ | ------ |
| 1 | Giulia |
| 2 | Michael |

| TeacherId | Name | 
| ------ | ------ |
| 1 | Jack |
| 2 | Emma |

| `TeacherId` | `StudentId` | 
| ------ | ------ |
| 1 | 1 |
| 1 | 2 |
| 2 | 1 |

``` cs
public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Teacher> Teachers { get; set; } = new ();
    }
```

```cs
public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Student> Students { get; set; } = new();
    }
```

---
## AddData
> You can use lines of code similar to the following to add related data in a `One-to-Many` relationship. 
    In the code below we have added the product object with the category. We don't need to add categories. Category is added automatically.
> You can also use lines of code like below to add data to the `One-to-One` and `Many-to-Many` relationship.

```cs
using (var _context = new AppDbContext())
{
    var newProduct = new Product
    {
        Name = "New Product",
        Price = 100,
        Category = new Category{Name = "New Category"}
    };
    _context.Products.Add(newProduct);
    _context.SaveChanges();
}

//Or

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
```

# EFCore Notlarý
## DbContext 
---
**Add/AddAsync**
**Update**
**Remove**
**Find/FindAsync**
**SaveChanges/SaveChangesAsync**
---
> Context üzerinde **'Add-Update-Remove'** iþlemleri yapýlýnca database üzerine hemen yansýtýlmaz.
> Memory de Added, Modified, Deleted olarak state ini deðiþtirir.
> SaveChanges veya SaveChangesAsync metodu ile database e yansýtýlýr.
---
> Note: SaveChanges yapýlana kadar yapýlan bütün deðiþiklikler memory de tutulur.
> SaveChanges yapýlýnca memorydeki deðiþikliklerden birinde hata olursa bütün deðiþiklikler geri alýnýr.
---
## Entry States
**Unchanged** - Veriyi ilk çektiðimizde `var products = await _context.Products.ToListAsync();` olan durumudur.

**Added** - Veriyi eklediðimizde olan durumdur. 
- `var product = new Product { Name = "Product 1" }	;` Burada 'detached' durumundadýr.
- `await _context.Products.AddAsync(product);` Burada 'added' durumuna geçer.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna geçer.

**Detached** - Veriyi oluþturduðumuzda fakat context e eklemediðimizde olan durumudur.
- `var product = new Product { Name = "Product 1" }	;` Burada 'detached' durumundadýr.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna geçer.

**Deleted** - Veriyi sildiðimizde olan durumudur.
- `var product = await _context.Products.FindAsync(1);` Burada 'unchanged' durumundadýr.
- `await _context.Products.RemoveAsync(product);` Burada 'deleted' durumuna geçer.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna geçer.

**Modified** - Veriyi güncellediðimizde olan durumudur.
- `var product = await _context.Products.FindAsync(1);` Burada 'unchanged' durumundadýr.
- `product.Name = "Kalem";` Burada 'modified' durumuna geçer.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna geçer.
---
## ChangeTracker
**ChangeTracker** - Context üzerindeki takip edilen entitleri tutar.
- `var changeTracker = _context.ChangeTracker.Entries();` Burada context üzerindeki deðiþiklikleri takip eder.
- `var changeTracker = _context.ChangeTracker.Entries<Product>();` Burada context üzerindeki Product tipindeki deðiþiklikleri takip eder.
- `var changeTracker = _context.ChangeTracker.Entries<Product>().ToList();` Burada context üzerindeki Product tipindeki deðiþiklikleri listeler.
- `var changeTracker = _context.ChangeTracker.Entries<Product>().Where(x => x.State == EntityState.Modified).ToList();` Burada context üzerindeki Product tipindeki deðiþiklikleri listeler ve durumu modified olanlarý getirir.
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
> Aþaðýda kurduðumuz iliþkide `Product` parent `ProductFeatureId` child olarak tanýmlanmýþtýr.
> `One-to-One` iliþkide `foreign key`  koyulan entity `child` olan entity olmalýdýr.

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
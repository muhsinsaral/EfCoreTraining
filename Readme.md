# EFCore Notlar�
## DbContext 
---
**Add/AddAsync**
**Update**
**Remove**
**Find/FindAsync**
**SaveChanges/SaveChangesAsync**
---
> Context �zerinde **'Add-Update-Remove'** i�lemleri yap�l�nca database �zerine hemen yans�t�lmaz.
> Memory de Added, Modified, Deleted olarak state ini de�i�tirir.
> SaveChanges veya SaveChangesAsync metodu ile database e yans�t�l�r.
---
> Note: SaveChanges yap�lana kadar yap�lan b�t�n de�i�iklikler memory de tutulur.
> SaveChanges yap�l�nca memorydeki de�i�ikliklerden birinde hata olursa b�t�n de�i�iklikler geri al�n�r.
---
## Entry States
**Unchanged** - Veriyi ilk �ekti�imizde `var products = await _context.Products.ToListAsync();` olan durumudur.

**Added** - Veriyi ekledi�imizde olan durumdur. 
- `var product = new Product { Name = "Product 1" }	;` Burada 'detached' durumundad�r.
- `await _context.Products.AddAsync(product);` Burada 'added' durumuna ge�er.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna ge�er.

**Detached** - Veriyi olu�turdu�umuzda fakat context e eklemedi�imizde olan durumudur.
- `var product = new Product { Name = "Product 1" }	;` Burada 'detached' durumundad�r.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna ge�er.

**Deleted** - Veriyi sildi�imizde olan durumudur.
- `var product = await _context.Products.FindAsync(1);` Burada 'unchanged' durumundad�r.
- `await _context.Products.RemoveAsync(product);` Burada 'deleted' durumuna ge�er.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna ge�er.

**Modified** - Veriyi g�ncelledi�imizde olan durumudur.
- `var product = await _context.Products.FindAsync(1);` Burada 'unchanged' durumundad�r.
- `product.Name = "Kalem";` Burada 'modified' durumuna ge�er.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna ge�er.
---
## ChangeTracker
**ChangeTracker** - Context �zerindeki takip edilen entitleri tutar.
- `var changeTracker = _context.ChangeTracker.Entries();` Burada context �zerindeki de�i�iklikleri takip eder.
- `var changeTracker = _context.ChangeTracker.Entries<Product>();` Burada context �zerindeki Product tipindeki de�i�iklikleri takip eder.
- `var changeTracker = _context.ChangeTracker.Entries<Product>().ToList();` Burada context �zerindeki Product tipindeki de�i�iklikleri listeler.
- `var changeTracker = _context.ChangeTracker.Entries<Product>().Where(x => x.State == EntityState.Modified).ToList();` Burada context �zerindeki Product tipindeki de�i�iklikleri listeler ve durumu modified olanlar� getirir.
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
> A�a��da kurdu�umuz ili�kide `Product` parent `ProductFeatureId` child olarak tan�mlanm��t�r.
> `One-to-One` ili�kide `foreign key`  koyulan entity `child` olan entity olmal�d�r.

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
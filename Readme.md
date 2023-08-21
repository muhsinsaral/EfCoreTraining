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

---

## Delete Behaviors
### `Cascade` (Default)
- EFCore implements this behavior by default
- If you delete a data, other data dependent on that data are also deleted.
- For example, you have product and category entities and there is a One-to-Many relationship between them. If you delete a category, all products belonging to that category are also deleted.
- ```cs
    protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Category>()
                    .HasMany(c => c.Products)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade); //Default
    
                base.OnModelCreating(modelBuilder);
            }
    ```

### `Restrict` 
- If you want to delete a data and there are child items under that data, it prevents the data from being deleted.
- ```cs
    protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Category>()
                    .HasMany(c => c.Products)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict); //Default
    
                base.OnModelCreating(modelBuilder);
            }
    ```
### `SetNull` 
- If you delete the parent entity, the foreign key in the child entities is set to `Null`.
- > Foreign Key must be nullable.
- ```cs
    protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Category>()
                    .HasMany(c => c.Products)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull); //Default
    
                base.OnModelCreating(modelBuilder);
            }
    ```

### `NoAction` 
- When we want `EfCore` to do nothing
- ```cs
    protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Category>()
                    .HasMany(c => c.Products)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction); //Default
    
                base.OnModelCreating(modelBuilder);
            }
    ```
---

## Releated Data Load
### `Eager Loading` 
- If we want to pull data from the Entities defined as `Navigation Property` while pulling data from the database with EFCore, we should use `Include` and `ThenInclude`.
- ```cs
    using (var _context = new AppDbContext())
    {
        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync();
        Console.WriteLine(product.Category.Name);
    }
    ```
    > If we want to pull `CategoryDetailst` then we can use `ThenInclude`
- ```cs
    using (var _context = new AppDbContext())
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .ThenInclude(c => c.CategoryDetailst)
            .FirstOrDefaultAsync();
        Console.WriteLine(product.Category.Name);
    }
    ```
    
### `Explicit Loading` 
- If we do not want to load the associated data when we first pull the entity, we can use it if we want to load it later if we need it.
- ```cs
    using (var _context = new AppDbContext())
    {
    
        //One to One
        var product = await _context.Products.FirstOrDefaultAsync();
        if (true)
        {
            _context.Entry(product).Reference(x => x.Category).Load();
            Console.WriteLine(product.Category.Name);
        }
    
        //One to Many
        var category = await _context.Categories.FirstOrDefaultAsync();
        if (true)
        {
            _context.Entry(category).Collection(x => x.Products).Load();
            foreach (var item in category.Products)
            {
                Console.WriteLine(item.Name);
            }
        }
    }
    ```


### `Lazy Loading` 
- If you are going to use lazy loading, you must first load the `Microsoft.EntityFrameworkCore.Proxies` library.
- You need to `enable lazy loading` and mark your navigation properties with `virtual`. 
- Now child entities will come from Db only when you call them.
- ```cs
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies()
                          .UseSqlServer(Initializer.Configuration.GetConnectionString("SqlCon"));
        }
    ```

- ```cs
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
    ```
---
## Inheritance
### `TPH (Table-per-Hierarchy)`
-  If a class is `inheriting` another class, the following scenarios can happen.
- ```cs
    public class BasePerson
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
     public class Manager : BasePerson
    {
        public int Grade { get; set; }
    }
    public class Employee : BasePerson
    {
        public decimal Salary { get; set; }
    }
    ```
 
    ---
    
- ```cs
    public class AppDbContext : DbContext
    {
        DbSet<Manager> Managers { get; set; }
        DbSet<Employee> Employees { get; set; }
        //...
    }
    ```
- > In the above case, `2 tables are created` in the Db.
-   | Id | Grade | FirstName | LastName |
    |------|------|------|------|
    | 1 | Senior Manager | Adam | Lively |
-   | Id | Salary  | FirstName | LastName |
    |------|------|------|------|
    | 1 | 6000 | Brendon | Fisher |

    ---
    
- ```cs
    public class AppDbContext : DbContext
    {
        DbSet<BasePerson> Persons { get; set; }
        DbSet<Manager> Managers { get; set; }
        DbSet<Employee> Employees { get; set; }
        //...
    }
    ```
- > In the above case, `1 tables are created` in the Db.
-   | Id | FirstName | LastName | Discriminator | Salary | Grade |
    |------|------|------|------|------|------|
    | 1 | Adam | Lively | Manager | NULL | Senior Manager |
    | 1 | Brendon | Fisher | Employee | 6000 | NULL |

### `TPT  (Table-per-Type)`
- If we do not want to combine two entities as above and create them in one table.
- If we want EF Core to create a different table with Many-to-Many in this case, we can use `ToTable`.
- ```cs
    public class BasePerson
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
     public class Manager : BasePerson
    {
        public int Grade { get; set; }
    }
    public class Employee : BasePerson
    {
        public decimal Salary { get; set; }
    }
    ```
- ```cs
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //TPT
            modelBuilder.Entity<BasePerson>().ToTable("Persons");
            modelBuilder.Entity<Manager>().ToTable("Managers");
            modelBuilder.Entity<Employee>().ToTable("Employees");

            base.OnModelCreating(modelBuilder);
        }
    ```
- > In the above case, `3 tables are created` in the Db.
-   | Id  | FirstName | LastName |
    |------|------|------|
    | 1 | Adam | Lively |
    | 2 | Brendon | Fisher |
-   | Id | Salary  | 
    |------|------|
    | 2 | 6000 | 
-   | Id | Grade  | 
    |------|------|
    | 1 | Senior Manager | 
---
## Model
### Entity Properties
-   `Keyless Entity Types`

    --  If we do not want the tables created in the db to have a primary key, we can use HasNoKey().
    ```cs
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasNoKey();
            //...
        }
    ```
    
-   `IsUnicode(false)`

    --  If we want to create `VARCHAR` strings instead of `NVARCHAR` by default, we can use it.
    > It can be used for fields such as `URL`, `UserName`.
    
    ```cs
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BasePerson>().Property(p => p.FirstName).IsUnicode(false);
            //...
        }
    ```
    
-   `HasColumnType()`

    --  We can use it especially if we want to specify the type in Db.
    
    ```cs
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BasePerson>().Property(p => p.FirstName).HasColumnType("nvarchar(50)");
            //...
        }
    ```
### Indexes
>   [What is Indexes?][Index] 
-   `HasIndex()`
    ```cs
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasIndex(p => p.Name);
            //...
        }
    ```
-   `Composite Index`
    ```cs
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasIndex(p => new { p.Name, p.Price });
            //...
        }
    ```
-   `Included Columns`
    ```cs
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name)
                .IncludeProperties(p => new { p.Price, p.CreatedDate });
            //...
        }
    ```
### Constraints
-   `HasCheckConstraint()`
    ```cs
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             modelBuilder.Entity<Product>().HasCheckConstraint("CK_Product_Price", "[Price] > [DiscountPrice]");
             modelBuilder.Entity<Product>().HasCheckConstraint("CK_Product_Price", "[Price] > 0");
            //...
        }
    ```

[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job. There is no need to format nicely because it shouldn't be seen. Thanks SO - http://stackoverflow.com/questions/4823468/store-comments-in-markdown-syntax)
[Index]: <https://www.youtube.com/watch?v=wWf1fUQ9wSc>


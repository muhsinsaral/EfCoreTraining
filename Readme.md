# EFCore Notları
## DbContext 
---
**Add/AddAsync**
**Update**
**Remove**
**Find/FindAsync**
**SaveChanges/SaveChangesAsync**
---
> Context üzerinde **'Add-Update-Remove'** işlemleri yapılınca database üzerine hemen yansıtılmaz.
> Memory de Added, Modified, Deleted olarak state ini değiştirir.
> SaveChanges veya SaveChangesAsync metodu ile database e yansıtılır.
---
> Note: SaveChanges yapılana kadar yapılan bütün değişiklikler memory de tutulur.
> SaveChanges yapılınca memorydeki değişikliklerden birinde hata olursa bütün değişiklikler geri alınır.

## Entry States
**Unchanged** - Veriyi ilk çektiğimizde `var products = await _context.Products.ToListAsync();` olan durumudur.

**Added** - Veriyi eklediğimizde olan durumdur. 
- `var product = new Product { Name = "Product 1" }	;` Burada 'detached' durumundadır.
- `await _context.Products.AddAsync(product);` Burada 'added' durumuna geçer.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna geçer.

**Detached** - Veriyi oluşturduğumuzda fakat context e eklemediğimizde olan durumudur.
- `var product = new Product { Name = "Product 1" }	;` Burada 'detached' durumundadır.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna geçer.

**Deleted** - Veriyi sildiğimizde olan durumudur.
- `var product = await _context.Products.FindAsync(1);` Burada 'unchanged' durumundadır.
- `await _context.Products.RemoveAsync(product);` Burada 'deleted' durumuna geçer.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna geçer.

**Modified** - Veriyi güncellediğimizde olan durumudur.
- `var product = await _context.Products.FindAsync(1);` Burada 'unchanged' durumundadır.
- `product.Name = "Kalem";` Burada 'modified' durumuna geçer.
- `await _context.SaveChangesAsync();` Burada 'unchanged' durumuna geçer.

## ChangeTracker

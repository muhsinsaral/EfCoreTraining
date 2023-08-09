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

## ChangeTracker

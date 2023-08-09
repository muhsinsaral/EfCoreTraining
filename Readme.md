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

## ChangeTracker

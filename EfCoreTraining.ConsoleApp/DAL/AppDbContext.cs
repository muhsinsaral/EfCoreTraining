using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreTraining.ConsoleApp.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<BasePerson> Persons { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Initializer.Build();
            optionsBuilder
                //.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
                //.UseLazyLoadingProxies()
                .UseSqlServer(Initializer.Configuration.GetConnectionString("SqlCon"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductFeatures)
                .WithOne(pf => pf.Product)
                .HasForeignKey<ProductFeature>(pf => pf.Id);
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);//Default

            //TPT
            modelBuilder.Entity<BasePerson>().ToTable("Persons");
            modelBuilder.Entity<Manager>().ToTable("Managers");
            modelBuilder.Entity<Employee>().ToTable("Employees");

            //employes hasnokey
            //modelBuilder.Entity<Employee>().HasNoKey();

            //BasePerson FirstName hascolumntype nvarchar(50)
            modelBuilder.Entity<BasePerson>().Property(p => p.FirstName).HasColumnType("nvarchar(50)");

            //CompositeIndex
            modelBuilder.Entity<Product>().HasIndex(p => p.Name).IncludeProperties(p => new { p.Price, p.CreatedDate });

            //Constraints
            modelBuilder.Entity<Product>().HasCheckConstraint("CK_Product_Price", "[Price] > [DiscountPrice]");

            base.OnModelCreating(modelBuilder);
        }


        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ChangeTracker.Entries().ToList().ForEach(e =>
            {
                if (e.Entity is Product p)
                {
                    if (e.State == EntityState.Added || e.State == EntityState.Modified)
                        p.UpdatedDateDate = DateTime.Now;
                }
            });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}

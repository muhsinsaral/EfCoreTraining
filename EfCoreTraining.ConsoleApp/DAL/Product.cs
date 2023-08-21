using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreTraining.ConsoleApp.DAL
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDateDate { get; set; }
        public bool IsDeleted { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ProductFeature? ProductFeatures { get; set; }
    }
}

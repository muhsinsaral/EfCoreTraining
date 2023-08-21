using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreTraining.ConsoleApp.DAL
{
    public class Employee : BasePerson
    {
        public decimal Salary { get; set; }
    }
}

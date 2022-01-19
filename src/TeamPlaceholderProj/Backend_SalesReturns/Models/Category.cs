using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_SalesReturns.Models
{
    public class Category
    {
        public int CategoryID;
        public string Description;
        //public List<Parts> Parts;
    }

    public record PartMenu { public int PartID; public string Description; public int CategoryID; public bool Discontinued;  }
}

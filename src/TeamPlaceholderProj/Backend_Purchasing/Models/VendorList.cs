using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Purchasing.Models
{
    /// <summary>
    /// Represents information for vendor in dropdown list
    /// </summary>
    public record VendorList(int VendorID, string VendorName)
    {
        public VendorList() : this(0, null) { }
    }

}

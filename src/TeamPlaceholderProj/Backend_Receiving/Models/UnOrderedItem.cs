using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Receiving.Models
{
    public record UnOrderedItem(string Description, string VendorPartId, int Qty);
    
}

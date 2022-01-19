using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Purchasing.Models
{
    /// <summary>
    /// Represents information of a part ordered by employee
    /// </summary>
    public record OrderedPart(int PurchaseOrderDetailID, int PartID, int QtyToOrder, decimal PurchasePrice);
}

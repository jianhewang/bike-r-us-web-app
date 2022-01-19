using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Receiving.Models
{
    public record CurrentReceiving(int PurchaseOrderDetailId, int RecQty, int Return, string Reason);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Receiving.Models
{
    public class OutstandingOrder
    {
        public int Id { get; set; }
        public int PoNumber { get; set; }
        public DateTime Date { get; set; }
        public string Vendor { get; set; }
        public string Phone { get; set; }
    }
        
    
}

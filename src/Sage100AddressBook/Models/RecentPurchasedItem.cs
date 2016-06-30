using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Models
{ 
public class RecentPurchasedItem : Sage100BaseEntity
    {
        public string ItemCode { get; set; }
        public string ItemCodeDesc { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double QuantityShipped { get; set; }
        public double UnitPrice { get; set; }
        public string UnitOfMeasure { get; set; }
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Models
{
    class Item : Sage100BaseEntity
    {
        public string ItemCode { get; set; }
        public string ItemCodeDesc { get; set; }
        public string CommentText { get; set; }
        public string UnitOfMeasure { get; set; }
        public string TaxClass { get; set; }
        public double StandardPrice { get; set; }
        public double RetailPrice { get; set; }
        public double QuantityOnHand { get; set; }
        public double quantityToBuy { get; set; }

    }
}

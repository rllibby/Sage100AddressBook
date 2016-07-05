using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Models
{
    class Order : OrderSummary
    {
        //to-do - add additional fields required for order that are not in OrderSummary
        public List<OrderDetail> Details { get; set; }

        public Order()
        {
            Details = new List<OrderDetail>();
        }
    }


    class OrderDetail : Sage100BaseEntity
    {
        public string ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemCodeDesc { get; set; }
        public string UnitOfMeasure { get; set; }
        public double QuantityOrdered { get; set; }
        public double UnitPrice { get; set; }
        public double ExtensionAmt { get; set; }
        public string LineSeqNo { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Models
{
    public class OrderSummary


    {
        public string SalesOrderNo { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public DateTime ShipExpireDate { get; set; }
        public string BillToName { get; set; }
        public double TaxableAmt { get; set; }
        public double NonTaxableAmt { get; set; }
        public double SalesTaxAmt { get; set; }
        public double DiscountAmt { get; set; }
        public double Total { get; set; }

    }

}

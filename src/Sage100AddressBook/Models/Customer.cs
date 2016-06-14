using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Models
{
    public class Customer : Sage100BaseEntity
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Telephone { get; set; }
        public string EmailAddress { get; set; }
        public DateTimeOffset? DateEstablished { get; set; }
        public string CaptionCurrrent { get; set; }
        public double CurrentBalance { get; set; }
        public string CaptionAging1 { get; set; }
        public double AgingCategory1 { get; set; }
        public string CaptionAging2 { get; set; }
        public double AgingCategory2 { get; set; }
        public string CaptionAging3 { get; set; }
        public double AgingCategory3 { get; set; }
        public string CaptionAging4 { get; set; }
        public double AgingCategory4 { get; set; }
        public DateTimeOffset? DateLastPayment { get; set; }
        public DateTimeOffset? DateLastStatemtent { get; set; }
        public double CreditLimit { get; set; }
        public double OpenOrderAmt { get; set; }
        public double AmountDue { get; set; }
        public double CreditRemaining { get; set; }
    }
}

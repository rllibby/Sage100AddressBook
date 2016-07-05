using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Models
{
    class SendQuoteMessage
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public string EmailAddress { get; set; }
    }
}

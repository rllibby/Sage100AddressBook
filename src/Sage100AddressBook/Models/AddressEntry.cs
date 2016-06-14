using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Models
{
    public class AddressEntry
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string PhoneRaw { get; set; }
        public string EmailAddress { get; set; }
        public string Type { get; set; }
        public string ParentId { get; set; }

    }

    public class AddressGroup
    {
        public string GroupName { get; set; }
        public List<AddressEntry> AddressEntries { get; set; }
    }
}

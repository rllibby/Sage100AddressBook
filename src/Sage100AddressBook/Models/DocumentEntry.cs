using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Models
{
    public class DocumentEntry
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
    }

    public class DocumentGroup
    {
        public string GroupName { get; set; }
        public List<DocumentEntry> DocumentEntries { get; set; }
    }

    public class DocumentFolder
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

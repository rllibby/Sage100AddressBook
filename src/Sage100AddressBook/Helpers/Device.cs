using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace Sage100AddressBook.Helpers
{
    public static class Device
    {
        public static bool IsMobile
        {
            get { return ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"); }
        }

    }
}

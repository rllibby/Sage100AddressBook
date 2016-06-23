using System;
/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Windows.Foundation.Metadata;

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Device helper class.
    /// </summary>
    public static class Device
    {
        /// <summary>
        /// Returns true if running on mobile device.
        /// </summary>
        public static bool IsMobile
        {
            get { return ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"); }
        }
    }
}

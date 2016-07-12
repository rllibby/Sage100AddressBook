/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Navigation event argument.
    /// </summary>
    public class NavigationArgs
    {
        /// <summary>
        /// The id of the entry.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The company code for the entry
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// The page was launched into directly via a protocol uri.
        /// </summary>
        public bool ProtocolLaunch { get; set; }
    }
}

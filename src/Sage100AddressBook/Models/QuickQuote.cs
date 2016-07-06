/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// POCO class for quick quote.
    /// </summary>
    public class QuickQuote
    {
        #region Public properties

        /// <summary>
        /// The customer id.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// The item identifier.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// The item quantity.
        /// </summary>
        public double Quantity { get; set; }

        #endregion
    }
}

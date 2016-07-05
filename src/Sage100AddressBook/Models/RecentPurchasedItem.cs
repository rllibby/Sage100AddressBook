/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;

namespace Sage100AddressBook.Models
{ 
    /// <summary>
    /// POCO class for the recently purchased items.
    /// </summary>
    public class RecentPurchasedItem : Sage100BaseEntity
    {
        #region Public properties

        /// <summary>
        /// The item code.
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// The item code description.
        /// </summary>
        public string ItemCodeDesc { get; set; }

        /// <summary>
        /// The invoice date.
        /// </summary>
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// The quantity shipped.
        /// </summary>
        public double QuantityShipped { get; set; }

        /// <summary>
        /// The unit price.
        /// </summary>
        public double UnitPrice { get; set; }

        /// <summary>
        /// The unit of measure.
        /// </summary>
        public string UnitOfMeasure { get; set; }

        #endregion
    }
}



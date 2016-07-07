/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using System;

namespace Sage100AddressBook.Models
{ 
    /// <summary>
    /// POCO class for the recently purchased items.
    /// </summary>
    public class RecentPurchasedItem
    {
        #region Private fields

        private string _description;
        private string _uom;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the dynamic width based on the device being displayed.
        /// </summary>
        public int ItemWidth
        {
            get
            {
                if (Device.IsMobile)
                {
                    var bounds = App.Bounds;

                    return Convert.ToInt32(bounds.Width - 30);
                }

                return 400;
            }
        }

        /// <summary>
        /// The item code.
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// The item code description.
        /// </summary>
        public string ItemCodeDesc
        {
            get { return string.IsNullOrEmpty(_description) ? "(Blank)" : _description; }
            set { _description = value; }
        }

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
        public string UnitOfMeasure
        {
            get { return _uom; }
            set { _uom = value?.ProperCase(); }
        }

        #endregion
    }
}



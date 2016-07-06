/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;

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

    /// <summary>
    /// POCO class for quick quote line editing.
    /// </summary>
    public class QuickQuoteLine
    {
        #region Private fields

        private int _quantity;

        #endregion

        #region Public properties

        /// <summary>
        /// Event handler for quantity changed.
        /// </summary>
        public EventHandler QuantityChanged;

        /// <summary>
        /// The item quantity.
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    QuantityChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// The item identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The item description.
        /// </summary>
        public string Description { get; set; }

        #endregion
    }
}

/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Enumeration for order identification.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Quote type.
        /// </summary>
        Quote,

        /// <summary>
        /// Order type.
        /// </summary>
        Order
    }

    /// <summary>
    /// Class for passing quote/order item for editing.
    /// </summary>
    public class QuoteOrderArgs
    {
        #region Public properties

        /// <summary>
        /// Type identification. 
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// The quote or order id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The customer id for the quote or order.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// The company id for the customer.
        /// </summary>
        public string CompanyId { get; set; }

        #endregion
    }
}

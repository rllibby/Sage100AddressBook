/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Class for passing quote/order item for editing.
    /// </summary>
    public class QuoteOrderArgs
    {
        #region Public properties

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

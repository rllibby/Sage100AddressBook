/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// POCO class for send quote message.
    /// </summary>
    class SendQuoteMessage
    {
        /// <summary>
        /// The order identifier.
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// The customer id.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// The email address to send the quote to.
        /// </summary>
        public string EmailAddress { get; set; }
    }
}

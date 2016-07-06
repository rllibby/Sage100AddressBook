/*
 *  Copyright © 2016, Sage Software, Inc. 
 */
using System.Collections.Generic;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// POCO class for full order.
    /// </summary>
    public class Order : OrderSummary
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public Order()
        {
            Details = new List<OrderDetail>();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The details for the order.
        /// </summary>
        public List<OrderDetail> Details { get; set; }

        #endregion
    }

    /// <summary>
    /// POCO class for an order line.
    /// </summary>
    public class OrderDetail : Sage100BaseEntity
    {
        #region Public properties

        /// <summary>
        /// The item identifier.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// The item code.
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// The item description.
        /// </summary>
        public string ItemCodeDesc { get; set; }

        /// <summary>
        /// The unit of measure for the item.
        /// </summary>
        public string UnitOfMeasure { get; set; }
       
        /// <summary>
        /// The quantity ordered.
        /// </summary>
        public double QuantityOrdered { get; set; }

        /// <summary>
        /// The unit price for the item.
        /// </summary>
        public double UnitPrice { get; set; }

        /// <summary>
        /// The total amount for the line
        /// </summary>
        public double ExtensionAmt { get; set; }

        /// <summary>
        /// The line sequence number.
        /// </summary>
        public string LineSeqNo { get; set; }

        #endregion
    }
}

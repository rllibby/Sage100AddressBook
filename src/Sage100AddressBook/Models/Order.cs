/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Model class for full quote/order.
    /// </summary>
    public class Order : OrderSummary
    {
        #region Private fields

        private List<OrderDetail> _details = new List<OrderDetail>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public Order() { }

        #endregion

        #region Public properties

        /// <summary>
        /// The details for the order.
        /// </summary>
        public List<OrderDetail> Details
        {
            get { return _details; }
            set { _details = value; }
         }

        #endregion
    }

    /// <summary>
    /// Model class for an order line.
    /// </summary>
    public class OrderDetail : Sage100BaseEntity
    {
        #region Private fields

        private double _quantityOrdered;
        private string _description;
        public bool _persisted;
        public bool _modified;

        #endregion

        #region Public properties

        /// <summary>
        /// Event handler for quantity ordered changed.
        /// </summary>
        [JsonIgnore]
        public EventHandler QuantityOrderedChanged;

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
        public string ItemCodeDesc
        {
            get { return (string.IsNullOrEmpty(_description) ? ItemCode : _description); }
            set { _description = value; }
        }

        /// <summary>
        /// The unit of measure for the item.
        /// </summary>
        public string UnitOfMeasure { get; set; }
       
        /// <summary>
        /// The quantity ordered.
        /// </summary>
        public double QuantityOrdered
        {
            get { return _quantityOrdered; }
            set
            {
                Modified = true;
                _quantityOrdered = value;

                QuantityOrderedChanged?.Invoke(this, new EventArgs());
            }
        }

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

        /// <summary>
        /// True if this exists in the back office.
        /// </summary>
        [JsonIgnore]
        public bool Persisted
        {
            get { return _persisted; }
            set { _persisted = value; }
        }

        /// <summary>
        /// True if this was added or modified.
        /// </summary>
        [JsonIgnore]
        public bool Modified
        {
            get { return _modified; }
            set { _modified = value; }
        }

        #endregion
    }
}

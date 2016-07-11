/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Helpers;
using System;
using Template10.Mvvm;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// POCO class for quote and order summary.
    /// </summary>
    public class OrderSummary : Sage100BaseEntity
    {
        #region Public properties

        /// <summary>
        /// Gets the dynamic width based on the device being displayed.
        /// </summary>
        [JsonIgnore]
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
        /// The quote or order number.
        /// </summary>
        public string SalesOrderNo { get; set; }

        /// <summary>
        /// The order or quote type.
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// The quote or order status.
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// The order ship or quote expiration date.
        /// </summary>
        public DateTime ShipExpireDate { get; set; }

        /// <summary>
        /// The bill to name.
        /// </summary>
        public string BillToName { get; set; }

        /// <summary>
        /// The taxable amount.
        /// </summary>
        public double TaxableAmt { get; set; }

        /// <summary>
        /// The non taxable amount.
        /// </summary>
        public double NonTaxableAmt { get; set; }

        /// <summary>
        /// The sales tax amount.
        /// </summary>
        public double SalesTaxAmt { get; set; }

        /// <summary>
        /// The discount amount.
        /// </summary>
        public double DiscountAmt { get; set; }

        /// <summary>
        /// The quote or order total.
        /// </summary>
        public double Total { get; set; }

        /// <summary>
        /// Delegate command for the delete action.
        /// </summary>
        [JsonIgnore]
        public DelegateCommand<OrderSummary> Delete { get; set; }

        /// <summary>
        /// Delegate command for the edit action.
        /// </summary>
        [JsonIgnore]
        public DelegateCommand<OrderSummary> Edit { get; set; }

        /// <summary>
        /// Delegate command for the send action.
        /// </summary>
        [JsonIgnore]
        public DelegateCommand<OrderSummary> Send { get; set; }

        #endregion
    }
}

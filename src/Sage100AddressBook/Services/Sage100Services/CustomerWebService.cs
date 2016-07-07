/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sage100AddressBook.Services.Sage100Services
{
    /// <summary>
    /// Class for obtaining the customer via a webapi call.
    /// </summary>
    public class CustomerWebService
    {
        #region Private fields

        private static CustomerWebService _instance = new CustomerWebService();

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the specified customer.
        /// </summary>
        /// <param name="custId">The customer id.</param>
        /// <param name="companyCode">The company code for the customer.</param>
        /// <returns>The customer on success, null if not found.</returns>
        public async Task<Customer> GetCustomerAsync(string custId, string companyCode)
        {
            if (string.IsNullOrEmpty(custId)) throw new ArgumentNullException("custId");
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");

            var content = await NgrokService.GetAsync(companyCode + "/Customers/" + custId);

            if (!string.IsNullOrEmpty(content)) return JsonConvert.DeserializeObject<Customer>(content);

            return JsonConvert.DeserializeObject<Customer>(OfflineData.CustomerData);
        }

        /// <summary>
        /// Gets the recently purchased items for the customer.
        /// </summary>
        /// <param name="custId">The customer id.</param>
        /// <param name="companyCode">The company code for the customer.</param>
        /// <returns>The collection of purchased items on success, null on failure.</returns>
        public async Task<IEnumerable<RecentPurchasedItem>> GetRecentlyPurchasedItemsAsync(string custId, string companyCode)
        {
            if (string.IsNullOrEmpty(custId)) throw new ArgumentNullException("custId");
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");

            var content = await NgrokService.GetAsync(companyCode + "/Customers/" + custId + "/RecentlyPurchasedItems");

            if (!string.IsNullOrEmpty(content)) return JsonConvert.DeserializeObject<List<RecentPurchasedItem>>(content);

            return new List<RecentPurchasedItem>();
        }

        /// <summary>
        /// Gets the quote summaries for the customer.
        /// </summary>
        /// <param name="custId">The customer id.</param>
        /// <param name="companyCode">The company code for the customer.</param>
        /// <returns>The collection of quote summaries on success, null on failure.</returns>
        public async Task<IEnumerable<OrderSummary>> GetQuotesSummaryAsync(string custId, string companyCode)
        {
            if (string.IsNullOrEmpty(custId)) throw new ArgumentNullException("custId");
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");

            var content = await NgrokService.GetAsync(companyCode + "/Customers/" + custId + "/Quotes");

            if (!string.IsNullOrEmpty(content)) return JsonConvert.DeserializeObject<List<OrderSummary>>(content);

            return JsonConvert.DeserializeObject<List<OrderSummary>>(OfflineData.QuoteSummaryData);
        }

        /// <summary>
        /// Gets the order summaries for the customer.
        /// </summary>
        /// <param name="custId">The customer id.</param>
        /// <param name="companyCode">The company code for the customer.</param>
        /// <returns>The collection of order summaries on success, null on failure.</returns>
        public async Task<IEnumerable<OrderSummary>> GetOrdersSummaryAsync(string custId, string companyCode)
        {
            if (string.IsNullOrEmpty(custId)) throw new ArgumentNullException("custId");
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");

            var content = await NgrokService.GetAsync(companyCode + "/Customers/" + custId + "/Orders");

            if (!string.IsNullOrEmpty(content)) return JsonConvert.DeserializeObject<List<OrderSummary>>(content);

            return JsonConvert.DeserializeObject<List<OrderSummary>>(OfflineData.OrderSummaryData);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The static instance to this service.
        /// </summary>
        public static CustomerWebService Instance
        {
            get { return _instance; }
        }

        #endregion
    }
}

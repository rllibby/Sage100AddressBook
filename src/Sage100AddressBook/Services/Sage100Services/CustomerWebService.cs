/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;

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

            var result = new Customer
            {
                CustomerId = "02-AUTOCR",
                CustomerName = "Autocraft Accessories",
                AddressLine1 = "310 Fernando Street",
                AddressLine2 = "",
                AddressLine3 = "",
                City = "Newport Beach",
                State = "CA",
                ZipCode = "92661-0002",
                Telephone = "(949) 555-1212",
                EmailAddress = "joe-bloggs@gmail.com",
                DateEstablished = new DateTimeOffset(2019, 01, 01, 0, 0, 0, new TimeSpan()),
                CaptionCurrrent = "Current",
                CurrentBalance = 12940.31,
                CaptionAging1 = "Over 30 Days",
                AgingCategory1 = 4657,
                CaptionAging2 = "Over 60 Days",
                AgingCategory2 = 6406.53,
                CaptionAging3 = "Over 90 Days",
                AgingCategory3 = 4607.18,
                CaptionAging4 = "Over 120 Days",
                AgingCategory4 = 980.34,
                DateLastPayment = new DateTimeOffset(2020, 05, 17, 0, 0, 0, new TimeSpan()),
                DateLastStatemtent = null,
                CreditLimit = 25000,
                OpenOrderAmt = 1908,
                AmountDue = 23954.02,
                CreditRemaining = -862.0200000000004,
                Id = "303141564E4554"
            };

            return result;
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

            return new List<OrderSummary>();
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

            return new List<OrderSummary>();
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

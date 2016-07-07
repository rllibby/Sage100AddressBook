/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sage100AddressBook.Services.SearchServices
{
    /// <summary>
    /// Search service for items.
    /// </summary>
    public class ItemSearchService
    {
        #region Private fields

        private static ItemSearchService _instance = new ItemSearchService();

        #endregion

        #region Public methods

        /// <summary>
        /// Performs an async search using the NGROK uri.
        /// </summary>
        /// <param name="companyCode">The company code to perform the search on.</param>
        /// <param name="customerId">The search string.</param>
        /// <returns>The collection of items.</returns>
        public async Task<IEnumerable<Item>> ExecuteSearchAsync(string companyCode, string searchString)
        {
            if (string.IsNullOrEmpty(companyCode) || string.IsNullOrEmpty(searchString)) return new List<Item>();

            var content = await NgrokService.GetAsync(companyCode + "/items?search=" + searchString);

            if (!string.IsNullOrEmpty(content)) return JsonConvert.DeserializeObject<List<Item>>(content);

            var result = new List<Item>();

            result.Add(new Item()
            {
                Id = "303141564E4554",
                ItemCode = "6655",
                ItemCodeDesc = "6.5 inch widget",
                UnitOfMeasure = "EACH",
                TaxClass = "TX",
                StandardPrice = 123.44,
                RetailPrice = 199.99,
                QuantityOnHand = 512,
                QuantityToBuy = 10
            });

            result.Add(new Item()
            {
                Id = "303141564E4554",
                ItemCode = "8953",
                ItemCodeDesc = "Universal 3 1/2\" SSDD flex",
                UnitOfMeasure = "EACH",
                TaxClass = "CA",
                StandardPrice = 113.44,
                RetailPrice = 120.50,
                QuantityOnHand = 42,
                QuantityToBuy = 3
            });

            return result;
        }

        /// <summary>
        /// Performs an async search using the NGROK uri.
        /// </summary>
        /// <param name="companyCode">The company code to perform the search on.</param>
        /// <param name="customerId">The customer id to get recommended items for.</param>
        /// <returns>The collection of items.</returns>
        public async Task<IEnumerable<Item>> ExecuteRecommendedAsync(string companyCode, string customerId)
        {
            if (string.IsNullOrEmpty(companyCode) || string.IsNullOrEmpty(customerId)) return new List<Item>();

            var content = await NgrokService.GetAsync(companyCode + "/customers/" + customerId + "/recommendeditems");

            if (!string.IsNullOrEmpty(content)) return JsonConvert.DeserializeObject<List<Item>>(content);

            var result = new List<Item>();

            result.Add(new Item()
            {
                Id = "303141564E4554",
                ItemCode = "6655",
                ItemCodeDesc = "6.5 inch widget",
                UnitOfMeasure = "EACH",
                TaxClass = "TX",
                StandardPrice = 123.44,
                RetailPrice = 199.99,
                QuantityOnHand = 512,
                QuantityToBuy = 1
            });

            result.Add(new Item()
            {
                Id = "303141564E4554",
                ItemCode = "8953",
                ItemCodeDesc = "Universal 3 1/2\" SSDD flex",
                UnitOfMeasure = "EACH",
                TaxClass = "CA",
                StandardPrice = 113.44,
                RetailPrice = 120.50,
                QuantityOnHand = 42,
                QuantityToBuy = 1
            });

            return result;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static ItemSearchService Instance
        {
            get { return _instance; }
        }

        #endregion
    }
}

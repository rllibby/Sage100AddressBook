using Newtonsoft.Json;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Services.SearchServices
{
    class ItemSearchService
    {
        #region Private constants

        private const string CompanyCode = "ABC";

        #endregion

        #region Private fields

        private static ItemSearchService _instance = new ItemSearchService();

        #endregion

        #region Public methods

        /// <summary>
        /// Performs an async search using the NGROK uri.
        /// </summary>
        /// <param name="baseUri">The base URI where NGROK is hosted.</param>
        /// <param name="searchString">The search string.</param>
        /// <returns>The collection of addresses.</returns>
        public async Task<IEnumerable<Item>> ExecuteSearchAsync(string searchString)
        {
            if (searchString == null) return new List<Item>();

            var content = await NgrokService.GetAsync(CompanyCode + "/items?search=" + searchString);

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
                quantityToBuy = 10

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

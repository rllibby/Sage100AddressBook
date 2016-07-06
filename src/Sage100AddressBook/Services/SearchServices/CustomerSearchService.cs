/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sage100AddressBook.Services.CustomerSearchServices
{
    /// <summary>
    /// Search service.
    /// </summary>
    public class CustomerSearchService
    {
        #region Private constants

        private const string CompanyCode = "abc"; 

        #endregion

        #region Private fields

        private static CustomerSearchService _instance = new CustomerSearchService();

        #endregion

        #region Public methods

        /// <summary>
        /// Performs an async search using the NGROK uri.
        /// </summary>
        /// <param name="baseUri">The base URI where NGROK is hosted.</param>
        /// <param name="searchString">The search string.</param>
        /// <returns>The collection of addresses.</returns>
        public async Task<IEnumerable<AddressEntry>> ExecuteSearchAsync(string searchString)
        {
            if (searchString == null) return new List<AddressEntry>();

            var content = await NgrokService.GetAsync(CompanyCode + "/addresses?search=" + searchString);

            if (!string.IsNullOrEmpty(content)) return JsonConvert.DeserializeObject<List<AddressEntry>>(content);

            return JsonConvert.DeserializeObject<List<AddressEntry>>(OfflineData.AddressSearchData);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static CustomerSearchService Instance
        {
            get { return _instance; }
        }

        #endregion
    }
}

/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Sage100AddressBook.Services.CustomerSearchServices
{
    /// <summary>
    /// Search service.
    /// </summary>
    public class CustomerSearchService
    {
        #region Private constants

        private const string CompanyCode = "ABC"; 

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

            var result = new List<AddressEntry>();

            result.Add(new AddressEntry()
            {
                Id = "303141564E4554",
                Name = "Adamson Plumbing Supply",
                Address = "123 Main Steet",
                City = "Irvine",
                State = "CA",
                ZipCode = "92614",
                Phone = "(949) 555-1323",
                EmailAddress = "adamson@gmail.com",
                PhoneRaw = "9495551323",
                Type = "Customer",
                ParentId = null
            });

            result.Add(new AddressEntry()
            {
                Id = "303141564E4554",
                Name = "McConaughey and Associates",
                Address = "123 Main Steet",
                City = "Bainbridge",
                State = "CA",
                ZipCode = "92614",
                Phone = "(949) 555-1323",
                PhoneRaw = "9495551323",
                Type = "Customer",
                ParentId = null
            });

            result.Add(new AddressEntry()
            {
                Id = "303141564E4550",
                Name = "Joe Mamma",
                Address = "123 Main Steet",
                City = "Irvine",
                State = "CA",
                ZipCode = "92614",
                Phone = "(949) 555-1323",
                EmailAddress = "jmamma@hotmail.com",
                PhoneRaw = "9495551323",
                Type = "Contact",
                ParentId = "303141564E4554"
            });

            return result;
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

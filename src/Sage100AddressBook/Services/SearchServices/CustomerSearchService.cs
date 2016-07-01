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
        public static CustomerSearchService Instance { get; } = new CustomerSearchService();
        private string compCode = "abc"; //to-do allow selection in settings

        /// <summary>
        /// Performs an async search using the NGROK uri.
        /// </summary>
        /// <param name="baseUri">The base URI where NGROK is hosted.</param>
        /// <param name="searchString">The search string.</param>
        /// <returns></returns>
        public async Task<IEnumerable<AddressEntry>> ExecuteSearchAsync(string baseUri, string searchString)
        {
            if (searchString == null) return new List<AddressEntry>();

#if (NGROK)
            using (var client = new HttpClient())
            {
                var searchURI = new Uri(baseUri + compCode + "/addresses?search=" + searchString);

                client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await client.GetAsync(searchURI))
                {
                    if (response.IsSuccessStatusCode == true)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<List<AddressEntry>>(content);
                    }
                }
            }

            return new List<AddressEntry>();
#else
            var result = new List<AddressEntry>();

            result.Add(new AddressEntry()
                    {
                        Id = "303141564E4550",
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
                        Id = "303141564E4554",
                        Name = "Joe Mamma",
                        Address = "123 Main Steet",
                        City = "Irvine",
                        State = "CA",
                        ZipCode = "92614",
                        Phone = "(949) 555-1323",
                        EmailAddress = "jmamma@hotmail.com",
                        PhoneRaw = "9495551323",
                        Type = "Contact",
                        ParentId = "123"
                    });
                }
            
            return result;
#endif
        }
    }
}

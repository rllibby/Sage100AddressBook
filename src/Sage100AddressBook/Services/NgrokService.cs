/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.Web.Http.Headers;

namespace Sage100AddressBook.Services
{
    /// <summary>
    /// Static class for making calls to ngrok web apis.
    /// </summary>
    public static class NgrokService
    {
        #region Private fields

        private static string _baseAddress = "https://sage100poc.ngrok.io/api/";

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the content at the specified address.
        /// </summary>
        /// <param name="address">The webapi address for the request.</param>
        /// <returns>The content on success, null on failure.</returns>
        public static async Task<string> GetAsync(string address)
        {
            if (address == null) throw new ArgumentNullException("address");

#if (NGROK)
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await client.GetAsync(new Uri((_baseAddress + address).ToLower())))
                {
                    if ((response != null) && (response.IsSuccessStatusCode == true))
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
#endif
            return await Task.FromResult<string>(null);
        }

        #endregion
    }
}

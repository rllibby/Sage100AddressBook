/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Sage100AddressBook.Services
{
    /// <summary>
    /// Static class for shortening URLs using Bitly.
    /// </summary>
    public static class BitlyService
    {
        #region Private fields

        private static string _baseAddress = "https://api-ssl.bitly.com/";
        private static string _apiKey = "f089eb10cd45c3c488f1fee396a0162fa2128464";

        #endregion

        #region Public methods

        /// <summary>
        /// Shortens the specified address using Bitly.
        /// </summary>
        /// <param name="address">The web api address for the request.</param>
        /// <returns>The shortened url on success, null on failure.</returns>
        public static async Task<string> ShortenUrl(string address)
        {
            if (address == null) throw new ArgumentNullException("address");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                try
                {
                    var url = WebUtility.UrlEncode(address);
                    var uri = new Uri(string.Format("{0}v3/shorten?access_token={1}&longUrl={2}&format=txt", _baseAddress, _apiKey, url));

                    using (var response = await client.GetAsync(uri))
                    {
                        if ((response != null) && (response.IsSuccessStatusCode == true))
                        {
                            return await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return await Task.FromResult<string>(null);
        }

        #endregion
    }
}

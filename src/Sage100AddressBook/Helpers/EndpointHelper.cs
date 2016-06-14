/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Helper class for obtaining json from endpoints.
    /// </summary>
    public static class EndpointHelper
    {
        #region Public methods

        /// <summary>
        /// Gets the json for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint url.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The json as string.</returns>
        public async static Task<string> GetJson(string endpoint, string accessToken)
        {
            if (string.IsNullOrEmpty(endpoint)) throw new ArgumentNullException("endpoint");
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException("accessToken");

            var uri = new Uri(string.Format("https://graph.microsoft.com/v1.0/{0}", endpoint));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                using (HttpResponseMessage response = await client.GetAsync(uri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }

                    return null;
                }
            }
        }

        #endregion
    }
}

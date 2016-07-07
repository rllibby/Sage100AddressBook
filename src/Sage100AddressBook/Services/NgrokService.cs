/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Services
{
    /// <summary>
    /// Static class for making calls to ngrok web apis.
    /// </summary>
    public static class NgrokService
    {
        #region Private fields

        private static string _baseAddress = "https://sage100poc.ngrok.io/api/";
        private static int _timeout = 5;

        #endregion

        #region Public methods

        /// <summary>
        /// Posts the content to the specified address.
        /// </summary>
        /// <param name="address">The web api address for the request.</param>
        /// <param name="payload">The object to serialize to json content.</param>
        /// <returns>The response content on success, null on failure.</returns>
        public static async Task<string> PostAsync(string address, object payload)
        {
            if (address == null) throw new ArgumentNullException("address");
            if (payload == null) throw new ArgumentException("payload");

#if (NGROK)
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
#if DEBUG
                client.Timeout = TimeSpan.FromSeconds(_timeout);
#endif
                try
                {
                    using (var response = await client.PostAsync(new Uri(_baseAddress + address), content))
                    {
                        if ((response != null) && (response.IsSuccessStatusCode == true))
                        {
                            return await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                catch (Exception exception)
                {
                    var cancelled = (exception as TaskCanceledException);

                    if (cancelled != null) _timeout = 1;
                }
            }
#endif
            return await Task.FromResult<string>(null);
        }

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
#if DEBUG
                client.Timeout = TimeSpan.FromSeconds(_timeout);
#endif
                try
                {
                    using (var response = await client.GetAsync(new Uri(_baseAddress + address)))
                    {
                        if ((response != null) && (response.IsSuccessStatusCode == true))
                        {
                            return await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                catch (Exception exception)
                {
                    var cancelled = (exception as TaskCanceledException);

                    if (cancelled != null) _timeout = 1;
                }
            }
#endif
            return await Task.FromResult<string>(null);
        }

        /// <summary>
        /// Deletes the content at the specified address.
        /// </summary>
        /// <param name="address">The webapi address for the request.</param>
        /// <returns>True on success, false on failure.</returns>
        public static async Task<bool> DeleteAsync(string address)
        {
            if (address == null) throw new ArgumentNullException("address");

#if (NGROK)
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
#if DEBUG
                client.Timeout = TimeSpan.FromSeconds(_timeout);
#endif
                try
                {
                    using (var response = await client.DeleteAsync(new Uri(_baseAddress + address)))
                    {
                        return response.IsSuccessStatusCode;
                    }
                }
                catch (Exception exception)
                {
                    var cancelled = (exception as TaskCanceledException);

                    if (cancelled != null) _timeout = 1;
                }
            }
#endif
            return false;
        }

        #endregion
    }
}

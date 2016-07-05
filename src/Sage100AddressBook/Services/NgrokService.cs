/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Sage100AddressBook.Services
{
    /// <summary>
    /// Static class for making calls to ngrok web apis.
    /// </summary>
    public static class NgrokService
    {
        #region Private fields

        private static string _baseAddress;

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the base address from the application resources.
        /// </summary>
        private static async Task LoadBaseAddress()
        {
            if (!string.IsNullOrEmpty(_baseAddress)) return;

            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _baseAddress = Application.Current.Resources["ngrok"].ToString();
            });
        }

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
            await LoadBaseAddress();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await client.GetAsync(new Uri(_baseAddress + address)))
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

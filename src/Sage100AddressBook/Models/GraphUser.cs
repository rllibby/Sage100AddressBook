/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// POCO class for Microsoft Graph user response.
    /// </summary>
    public class GraphUser : BindableBase
    {
        #region Public methods

        /// <summary>
        /// Gets the /me graph user from the the endpoint.
        /// </summary>
        /// <param name="token">The access token to use for authentication.</param>
        /// <returns>An instance of GraphUser.</returns>
        public async static Task<GraphUser> Get(string token = null)
        {
            return await EndpointHelper.GetObject<GraphUser>("/me", token);
        }

        /// <summary>
        /// Gets the /users/id|principal graph user from the data obtained from the endpoint.
        /// </summary>
        /// <param name="idOrPrincipal">The id or prinicipal for the user to get.</param>
        /// <param name="token">The access token to use for authentication.</param>
        /// <returns>An instance of GraphUser.</returns>
        public async static Task<GraphUser> Get(string idOrPrincipal, string token = null)
        {
            if (string.IsNullOrEmpty(idOrPrincipal)) throw new ArgumentNullException("idOrPrincipal");

            return await EndpointHelper.GetObject<GraphUser>(string.Format("/users/{0}", idOrPrincipal), token);
        }

        /// <summary>
        /// Gets the /users graph user collection from the data obtained from the endpoint.
        /// </summary>
        /// <param name="idOrPrincipal">The id or prinicipal for the user to get.</param>
        /// <param name="token">The access token to use for authentication.</param>
        /// <returns>An instance of GraphUser.</returns>
        public async static Task<ObservableCollection<GraphUser>> All(string token = null)
        {
            var collection = await EndpointHelper.GetObject<GraphUsers>("/users", token);

            return collection.Users;
        }

        /// <summary>
        /// Returns the string representation of the object.
        /// </summary>
        /// <returns>The string representation of the object.</returns>
        public override string ToString()
        {
            return DisplayName;
        }

        #endregion

        #region Public properties

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "givenName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "surname")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "userPrincipalName")]
        public string PrincipalName { get; set; }

        [JsonProperty(PropertyName = "mobilePhone")]
        public string MobilePhone { get; set; }

        #endregion
    }
}

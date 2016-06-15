/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// POCO class for Microsoft Graph user response.
    /// </summary>
    public class GraphUser
    {
        #region Public methods

        /// <summary>
        /// Returns the string representation of the object.
        /// </summary>
        /// <returns>The string representation of the object.</returns>
        public override string ToString()
        {
            return string.Format("(id={0}) {1}, {2}", Id ?? string.Empty, LastName ?? string.Empty, FirstName ?? string.Empty);
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

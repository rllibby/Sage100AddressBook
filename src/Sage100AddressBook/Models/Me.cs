/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// POCO class for Microsoft Graph /v1.0/Me response.
    /// </summary>
    public class Me
    {
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
    }
}

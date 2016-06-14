/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Class for handling POCO return on Graph users collection.
    /// </summary>
    public class GraphUsers
    {
        #region Private fields

        private List<GraphUser> _users = new List<GraphUser>();

        #endregion

        #region Public properties

        [JsonProperty(PropertyName = "value")]
        public List<GraphUser> Users
        {
            get { return _users; }
            set { _users = value; }
        }

        #endregion
    }
}

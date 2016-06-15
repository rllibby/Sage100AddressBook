/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Internal class for handling POCO return on Graph users collection.
    /// </summary>
    internal class GraphUsers
    {
        #region Private fields

        private ObservableCollection<GraphUser> _users = new ObservableCollection<GraphUser>();

        #endregion

        #region Public properties

        [JsonProperty(PropertyName = "value")]
        public ObservableCollection<GraphUser> Users
        {
            get { return _users; }
            set { _users = value; }
        }

        #endregion
    }
}

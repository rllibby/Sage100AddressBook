/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using System;
using System.Collections.Generic;
using Template10.Mvvm;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Model class for address entry.
    /// </summary>
    public class AddressEntry : BindableBase
    {
        /// <summary>
        /// The id of the entry.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Entity name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Entity address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Entity city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Entity state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Entity zip code.
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Entity phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Entity phone number in raw format.
        /// </summary>
        public string PhoneRaw { get; set; }

        /// <summary>
        /// Entity email address.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Entity type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Entity parent id.
        /// </summary>
        public string ParentId { get; set; }
    }

    /// <summary>
    /// Model for groups of address entities.
    /// </summary>
    public class AddressGroup : BindableBase
    {
        #region Private fields

        private ObservableCollectionEx<AddressEntry> _addressEntries = new ObservableCollectionEx<AddressEntry>();
        private string _groupName;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AddressGroup() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        public AddressGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) throw new ArgumentNullException("groupName");

            _groupName = groupName;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        /// <param name="collection">The collection to seed the group with.</param>
        public AddressGroup(string groupName, IEnumerable<AddressEntry> collection)
        {
            if (string.IsNullOrEmpty(groupName)) throw new ArgumentNullException("groupName");
            if (collection == null) throw new ArgumentNullException("collection");

            _groupName = groupName;
            _addressEntries.Set(collection);
        }

        #endregion

        /// <summary>
        /// The name of the group of address entities.
        /// </summary>
        public string GroupName
        {
            get { return _groupName; }
            set { Set(ref _groupName, value); }
        }

        /// <summary>
        /// Collection of address entities in the group.
        /// </summary>
        public ObservableCollectionEx<AddressEntry> AddressEntries
        {
            get { return _addressEntries; }
        }
    }
}

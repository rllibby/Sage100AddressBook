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
        #region Private fields

        private DelegateCommand<AddressEntry> _delete;
        private string _type;
        private string _address;

        #endregion

        #region Public properties

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
        public string Address
        {
            get { return _address; }
            set
            {
                _address = (string.IsNullOrEmpty(value) ? value : value.Replace("\n", ", "));

                base.RaisePropertyChanged();
            }
        }

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
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value.ProperCase();

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Entity parent id.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Delegate command for delete action.
        /// </summary>
        public DelegateCommand<AddressEntry> Delete
        {
            get { return _delete; }
            set { Set(ref _delete, value); }
        }

        #endregion
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
            set
            {
                _groupName = value.ProperCase();

                base.RaisePropertyChanged();
            }
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

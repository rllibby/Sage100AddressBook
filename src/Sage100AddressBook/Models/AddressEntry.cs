/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Helpers;
using System;
using System.Collections.Generic;
using Template10.Mvvm;
using Windows.UI.Xaml.Controls;

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

        #region Public methods

        /// <summary>
        /// Gets the navigation event arguments.
        /// </summary>
        /// <param name="entry">The address entry.</param>
        /// <param name="removePage">True if the receiver should remove this page from the back stack.</param>
        /// <returns>The navigation event args.</returns>
        public static NavigationArgs GetNavArgs(AddressEntry entry)
        {
            if (entry == null) throw new ArgumentNullException("entry");

            var nullTerm = entry.Id.IndexOf("00");
            var id = (nullTerm > 0) ? entry.Id.Substring(0, nullTerm) : entry.Id;

            return new NavigationArgs()
            {
                Id = (string.IsNullOrEmpty(entry.ParentId) ? id : entry.ParentId),
                CompanyCode = "abc",
                ProtocolLaunch = false
            };
        }
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
        [JsonIgnore]
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
        private string _groupType;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AddressGroup() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupType">The group type.</param>
        public AddressGroup(string groupType)
        {
            if (string.IsNullOrEmpty(groupType)) throw new ArgumentNullException("groupType");

            _groupType = groupType;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupType">The group type.</param>
        /// <param name="collection">The collection to seed the group with.</param>
        public AddressGroup(string groupType, IEnumerable<AddressEntry> collection)
        {
            if (string.IsNullOrEmpty(groupType)) throw new ArgumentNullException("groupType");
            if (collection == null) throw new ArgumentNullException("collection");

            _groupType = groupType;
            _addressEntries.Set(collection);
        }

        #endregion

        public string GroupType
        {
            get { return _groupType; }
            set
            {
                _groupType = value;

                RaisePropertyChanged("GroupName");
                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The name of the group of address entities.
        /// </summary>
        public string GroupName
        {
            get { return _groupType + "s"; }
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

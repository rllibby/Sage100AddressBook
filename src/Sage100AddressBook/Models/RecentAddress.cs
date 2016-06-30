/*
 *  Copyright © 2016, Sage Software, Inc.
 */

using Sage100AddressBook.Helpers;
using System;
using Template10.Mvvm;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Class for managing recent entries.
    /// </summary>
    public class RecentAddress
    {
        #region Private fields

        private ObservableCollectionEx<AddressGroup> _groups = new ObservableCollectionEx<AddressGroup>();
        private static DelegateCommand<AddressEntry> _delete;
        private static RecentAddress _instance = new RecentAddress();

        #endregion

        #region Private methods

        /// <summary>
        /// Removes the address entry from the collection.
        /// </summary>
        /// <param name="entry">The address entry to remove.</param>
        private void DeleteEntry(AddressEntry entry)
        {
            if (entry == null) return;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private RecentAddress()
        {
            _delete = new DelegateCommand<AddressEntry>(new Action<AddressEntry>(DeleteEntry));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Add a recent entry. If this entry already exists, then move the existing entry to the top of the group.
        /// </summary>
        /// <param name="entry"></param>
        public void AddRecent(AddressEntry entry)
        {
            if (entry == null) return;

            var copy = entry.Copy();

            copy.Delete = _delete;

            foreach (var group in _groups)
            {
                if (group.GroupName.Equals(copy.Type))
                {
                    for (var i = 0; i < group.AddressEntries.Count; i++)
                    {
                        var compare = group.AddressEntries[i];

                        if (string.Equals(compare.Id, copy.Id) && string.Equals(compare.Name, copy.Name) && string.Equals(compare.ParentId, copy.ParentId))
                        {
                            if (i == 0) return;

                            group.AddressEntries.Move(i, 0);

                            return;   
                        }
                    }

                    group.AddressEntries.Insert(0, copy);

                    return;
                }
            }

            _groups.Add(new AddressGroup(copy.Type, new[] { copy }));
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Collection of address groups.
        /// </summary>
        public ObservableCollectionEx<AddressGroup> Groups
        {
            get { return _groups; }
        }

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static RecentAddress Instance
        {
            get { return _instance; }
        }

        #endregion
    }
}

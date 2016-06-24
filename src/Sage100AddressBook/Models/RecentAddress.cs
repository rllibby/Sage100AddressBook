/*
 *  Copyright © 2016, Sage Software, Inc.
 */

using Sage100AddressBook.Helpers;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Class for managing recent entries.
    /// </summary>
    public class RecentAddress
    {
        #region Private fields

        private ObservableCollectionEx<AddressGroup> _groups = new ObservableCollectionEx<AddressGroup>();
        private static RecentAddress _instance = new RecentAddress();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private RecentAddress() { }

        #endregion

        #region Public methods

        /// <summary>
        /// Add a recent entry. If this entry already exists, then move the existing entry to the top of the group.
        /// </summary>
        /// <param name="entry"></param>
        public void AddRecent(AddressEntry entry)
        {
            if (entry == null) return;

            foreach (var group in _groups)
            {
                if (group.GroupName.Equals(entry.Type))
                {
                    for (var i = 0; i < group.AddressEntries.Count; i++)
                    {
                        var compare = group.AddressEntries[i];

                        if (string.Equals(compare.Id, entry.Id) && string.Equals(compare.Name, entry.Name) && string.Equals(compare.ParentId, entry.ParentId))
                        {
                            if (i == 0) return;

                            group.AddressEntries.Move(i, 0);

                            return;   
                        }
                    }

                    group.AddressEntries.Add(entry);

                    return;
                }
            }

            _groups.Add(new AddressGroup(entry.Type, new[] { entry }));
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

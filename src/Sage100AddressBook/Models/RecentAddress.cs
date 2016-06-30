/*
 *  Copyright © 2016, Sage Software, Inc.
 */

using Sage100AddressBook.Helpers;
using System;
using System.Linq;
using Template10.Mvvm;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Class for managing recent entries.
    /// </summary>
    public class RecentAddress : BindableBase
    {
        #region Private fields

        private ObservableCollectionEx<AddressGroup> _groups = new ObservableCollectionEx<AddressGroup>();
        private DelegateCommand<AddressEntry> _delete;
        private DelegateCommand _clear;
        private static RecentAddress _instance = new RecentAddress();

        #endregion

        #region Private methods

        /// <summary>
        /// Determines if the clear command should be enabled.
        /// </summary>
        /// <returns>True if the command can be executed.</returns>
        private bool CanClear()
        {
            return (_groups.Count > 0);
        }

        /// <summary>
        /// Clears the recent list.
        /// </summary>
        private void ClearAction()
        {
            try
            {
                _groups.Clear();
            }
            finally
            {
                OnEmpty();
            }
        }
        
        /// <summary>
        /// Update commands based on state.
        /// </summary>
        private void OnEmpty()
        {
            try
            {
                _clear.RaiseCanExecuteChanged();
            }
            finally
            {
                RaisePropertyChanged("IsEmpty");
            }
        }

        /// <summary>
        /// Removes the address entry from the collection.
        /// </summary>
        /// <param name="entry">The address entry to remove.</param>
        private void DeleteEntry(AddressEntry entry)
        {
            if (entry == null) return;

            try
            {
                foreach (var group in _groups)
                {
                    if (group.GroupType.Equals(entry.Type))
                    {
                        for (var i = 0; i < group.AddressEntries.Count; i++)
                        {
                            var compare = group.AddressEntries[i];

                            if (string.Equals(compare.Id, entry.Id) && string.Equals(compare.Name, entry.Name) && string.Equals(compare.ParentId, entry.ParentId))
                            {
                                group.AddressEntries.RemoveAt(i);

                                return;
                            }
                        }
                    }
                }
            }
            finally
            {
                var empty = _groups.Where(g => (g.AddressEntries.Count == 0));

                foreach (var group in empty.ToList()) _groups.Remove(group);

                if (_groups.Count == 0) OnEmpty();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private RecentAddress()
        {
            _delete = new DelegateCommand<AddressEntry>(new Action<AddressEntry>(DeleteEntry));
            _clear = new DelegateCommand(new Action(ClearAction), CanClear);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Add a recent entry. If this entry already exists, then move the existing entry to the top of the group.
        /// </summary>
        /// <param name="entry">The entry to add</param>
        public void AddRecent(AddressEntry entry)
        {
            if (entry == null) return;

            var copy = entry.Copy();

            copy.Delete = _delete;

            foreach (var group in _groups)
            {
                if (group.GroupType.Equals(copy.Type))
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

        /// <summary>
        /// Clear action.
        /// </summary>
        public DelegateCommand Clear
        {
            get { return _clear; }
        }

        /// <summary>
        /// Determines if the recent list is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return (_groups.Count == 0); }
        }

        #endregion
    }
}
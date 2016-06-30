/*
 *  Copyright © 2016, Sage Software, Inc.
 */

using Newtonsoft.Json;
using Sage100AddressBook.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Template10.Mvvm;
using Windows.Storage;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Class for managing favorite entries.
    /// </summary>
    public class FavoriteAddress : BindableBase
    {
        #region Private constants

        private const string Favorite = "Favorite";

        #endregion

        #region Private fields

        [JsonExtensionData]
        private Dictionary<string, AddressEntry> _entries = new Dictionary<string, AddressEntry>();
        private ObservableCollectionEx<AddressGroup> _groups = new ObservableCollectionEx<AddressGroup>();
        private DelegateCommand<AddressEntry> _delete;
        private static FavoriteAddress _instance = new FavoriteAddress();

        #endregion

        #region Private methods

        /// <summary>
        /// Update commands based on state.
        /// </summary>
        private void OnEmpty()
        {
            RaisePropertyChanged("IsEmpty");
        }

        /// <summary>
        /// Called when the collection state changes. We need this hook to persist the state to local storage.
        /// </summary>
        private void OnModified()
        {
            ApplicationData.Current.LocalSettings.Values[Favorite] = JsonConvert.SerializeObject(_entries); 
        }

        /// <summary>
        /// Determines if the entry can be deleted.
        /// </summary>
        /// <param name="entry">The entry to evaluate.</param>
        /// <returns>True if the entry can be deleted.</returns>
        private bool CanDelete(AddressEntry entry)
        {
            return (entry != null);
        }

        /// <summary>
        /// Removes the address entry from the collection.
        /// </summary>
        /// <param name="entry">The address entry to remove.</param>
        private void DeleteEntry(AddressEntry entry)
        {
            if (entry == null) return;

            if (_entries.ContainsKey(entry.Id))
            {
                _entries.Remove(entry.Id);
                OnModified();
            }

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
        private FavoriteAddress()
        {
            var serialized = ApplicationData.Current.LocalSettings.Values[Favorite] as string;

            _delete = new DelegateCommand<AddressEntry>(new Action<AddressEntry>(DeleteEntry), CanDelete);

            if (!string.IsNullOrEmpty(serialized))
            {
                try
                {
                    var temp = JsonConvert.DeserializeObject<Dictionary<string, AddressEntry>>(serialized);

                    foreach (var entry in temp)
                    {
                        entry.Value.Delete = _delete;

                        AddFavorite(entry.Value);
                    }
                }
                catch 
                {
                    ApplicationData.Current.LocalSettings.Values[Favorite] = null;
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines if the specified address id is in the favorites collection.
        /// </summary>
        /// <param name="id">The id of the address to lookup.</param>
        /// <returns>True if in the favorites collection, otherwise false.</returns>
        public bool IsFavorite(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;

            return (_entries.ContainsKey(id));
        }

        /// <summary>
        /// Adds or removes favorites based on state.
        /// </summary>
        /// <param name="entry">The entry to add or remove.</param>
        public void ToggleFavorite(AddressEntry entry)
        {
            if (entry == null) return;

            if (IsFavorite(entry.Id))
            {
                DeleteEntry(entry);
                return;
            }

            AddFavorite(entry);
        }

        /// <summary>
        /// Add a favorite entry. If this entry already exists, then move the existing entry to the top of the group.
        /// </summary>
        /// <param name="entry">The entry to add.</param>
        public void AddFavorite(AddressEntry entry)
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

                    _entries.Add(copy.Id, copy);
                    group.AddressEntries.Insert(0, copy);

                    OnModified();

                    return;
                }
            }

            _entries.Add(copy.Id, copy);
            _groups.Add(new AddressGroup(copy.Type, new[] { copy }));

            OnModified();
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
        public static FavoriteAddress Instance
        {
            get { return _instance; }
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

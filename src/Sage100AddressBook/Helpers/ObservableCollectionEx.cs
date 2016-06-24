/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Template10.Common;

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Extension class that allows for AddRange on the observable collection.
    /// </summary>
    /// <typeparam name="T">The type being held by the collection.</typeparam>
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        #region Private fields

        private int _suppressNotification;

        #endregion

        #region Private fields

        /// <summary>
        /// Copies elements from the source collection, optionally clearing items before doing so.
        /// </summary>
        /// <param name="collection">The collection to copy items from.</param>
        /// <param name="clearExisting">True if existing items should be cleared.</param>
        private void InternalSet(IEnumerable<T> collection, bool clearExisting)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            _suppressNotification++;

            try
            {
                if (clearExisting) Clear();

                foreach (var item in collection) Add(item);
            }
            finally
            {
                _suppressNotification--;
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Triggered when the collection changes.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_suppressNotification <= 0) base.OnCollectionChanged(e);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds the collection of T to our collection.
        /// </summary>
        /// <param name="collection">The collection to load the new items from.</param>
        /// <param name="notify">The dispatcher wrapper to used for thread access.</param>
        public void Append(IEnumerable<T> collection, DispatcherWrapper notify = null)
        {
            try
            {
                InternalSet(collection, false);
            }
            finally
            {
                Reset(notify);
            }
        }

        /// <summary>
        /// Clears the existing items and adds the collection of T to our collection.
        /// </summary>
        /// <param name="collection">The collection to load the new items from.</param>
        /// <param name="notify">The dispatcher wrapper to used for thread access.</param>
        public void Set(IEnumerable<T> collection, IDispatcherWrapper notify = null)
        {
            try
            {
                InternalSet(collection, true);
            }
            finally
            {
                Reset(notify);
            }
        }

        /// <summary>
        /// Locks the collection from updating.
        /// </summary>
        public void BeginUpdate()
        {
            _suppressNotification++;
        }

        /// <summary>
        /// Removes a lock count from the notification counter and fires a reset if the count is at zero.
        /// </summary>
        /// <param name="notify">The dispatcher wrapper to used for thread access.</param>
        public void EndUpdate(IDispatcherWrapper notify = null)
        {
            try
            {
                _suppressNotification--;
            }
            finally
            {
                Reset(notify);
            }
        }

        /// <summary>
        /// Fires a notification that the collection has been reset and should be queried for the changes.
        /// </summary>
        /// <param name="notify">The dispatcher wrapper to used for thread access.</param>
        public async void Reset(IDispatcherWrapper notify = null)
        {
            if (_suppressNotification > 0) return;

            if (notify == null)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                return;
            }

            await notify.DispatchAsync(() =>
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }

        #endregion
    }
}
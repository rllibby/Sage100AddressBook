﻿/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using System.Collections.Generic;

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Class for handling self expriring data.
    /// </summary>
    /// <typeparam name="T">The collection type being cached.</typeparam>
    internal class CacheData<T> where T : class
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="expires">The expiration time in minutes.</param>
        public CacheData(int expires)
        {
            Expires = DateTime.Now.AddMinutes(expires);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The cached data.
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// The expiration date time.
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Determines if the entry is expired.
        /// </summary>
        public bool IsExpired
        {
            get { return (Expires <= DateTime.Now); }
        }

        #endregion
    }

    /// <summary>
    /// Collection cache class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionCache<T> where T : class
    {
        #region Private fields

        private Dictionary<string, CacheData<T>> _cache = new Dictionary<string, CacheData<T>>();
        private static int _expiresInMinutes = 5;

        #endregion

        #region Private methods

        /// <summary>
        /// Builds the dictionary key.
        /// </summary>
        /// <param name="company">The company code used for the key.</param>
        /// <param name="customer">The customer id used for the key.</param>
        /// <returns>The dictionary key.</returns>
        private string GetKey(string company, string customer)
        {
            if (string.IsNullOrEmpty(company)) throw new ArgumentNullException("company");
            if (string.IsNullOrEmpty(customer)) throw new ArgumentNullException("customer");

            return string.Format("{0,10:}{1,30}", company.ToLower(), customer.ToLower());
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public CollectionCache()
        {
            _expiresInMinutes = 5;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="expiresInMinutes">The time toe expire entries, in minutes.</param>
        public CollectionCache(int expiresInMinutes)
        {
            _expiresInMinutes = Math.Max(expiresInMinutes, 1);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the cache data for the specified company and customer key.
        /// </summary>
        /// <param name="company">The company code used for the key.</param>
        /// <param name="customer">The customer id used for the key.</param>
        /// <param name="collection">The collection to cache.</param>
        public void Set(string company, string customer, IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            var key = GetKey(company, customer);

            if (_cache.ContainsKey(key)) _cache.Remove(key);

            var entry = new CacheData<T>(_expiresInMinutes);

            entry.Data = collection;

            _cache.Add(key, entry);
        }

        /// <summary>
        /// Gets the cached collection from the dictionary.
        /// </summary>
        /// <param name="company">The company code used for the key.</param>
        /// <param name="customer">The customer id used for the key.</param>
        /// <returns>The collection on success, null on failure.</returns>
        public IEnumerable<T> Get(string company, string customer)
        {
            var key = GetKey(company, customer);

            if (!_cache.ContainsKey(key)) return null;

            var entry = _cache[key];

            if (entry.IsExpired)
            {
                _cache.Remove(key);

                return null;
            }

            return entry.Data;
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Count of cached items.
        /// </summary>
        public int Count
        {
            get { return _cache.Count; }
        }

        #endregion
    }
}

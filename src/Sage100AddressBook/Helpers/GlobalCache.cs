/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Models;

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Global cache class for the entire application.
    /// </summary>
    public static class GlobalCache
    {
        #region Private fields

        private static CollectionCache<DocumentEntry> _documentCache = new CollectionCache<DocumentEntry>();
        private static CollectionCache<DocumentFolder> _folderCache = new CollectionCache<DocumentFolder>();
        private static CollectionCache<OrderSummary> _quoteCache = new CollectionCache<OrderSummary>();
        private static CollectionCache<OrderSummary> _orderCache = new CollectionCache<OrderSummary>();
        private static CollectionCache<RecentPurchasedItem> _recentCache = new CollectionCache<RecentPurchasedItem>(5);

        #endregion

        #region Public properties
        
        /// <summary>
        /// Document cache.
        /// </summary>
        public static CollectionCache<DocumentEntry> DocumentCache
        {
            get { return _documentCache; }
        }

        /// <summary>
        /// Folder cache.
        /// </summary>
        public static CollectionCache<DocumentFolder> FolderCache
        {
            get { return _folderCache; }
        }

        /// <summary>
        /// Quote cache.
        /// </summary>
        public static CollectionCache<OrderSummary> QuoteCache
        {
            get { return _quoteCache; }
        }

        /// <summary>
        /// Order cache.
        /// </summary>
        public static CollectionCache<OrderSummary> OrderCache
        {
            get { return _orderCache; }
        }

        /// <summary>
        /// Recent cache.
        /// </summary>
        public static CollectionCache<RecentPurchasedItem> RecentCache
        {
            get { return _recentCache; }
        }

        #endregion
    }
}

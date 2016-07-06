using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.Sage100Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Core;
using Windows.UI.Xaml;

#pragma warning disable 4014

namespace Sage100AddressBook.ViewModels
{
    public class RecentPurchasedPivotViewModel : BindableBase
    {
        #region Private constants

        private const int PivotIndex = 4;

        #endregion

        #region Private fields

        private ObservableCollectionEx<RecentPurchasedItem> _recentPurchased = new ObservableCollectionEx<RecentPurchasedItem>();
        private ViewModelLoading _owner;
        //private OrderSummary _current;
        private string _companyCode;
        private string _rootId;
        private int _index = (-1);
        private bool _loading;

        #endregion

        #region Private methods


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public RecentPurchasedPivotViewModel(ViewModelLoading owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saves the id and company code.
        /// </summary>
        /// <param name="id">The id for the entity.</param>
        /// <param name="companyCode">The company code.</param>
        public void SetArguments(string id, string companyCode)
        {
            _rootId = id;
            _companyCode = companyCode;
        }

        /// <summary>
        /// Loads the documents from the retrieval service.
        /// </summary>
        /// <returns>The async task to wait on.</returns>
        public async Task Load()
        {
            if (_index != PivotIndex) return;

            var dispatcher = Window.Current.Dispatcher;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Loading = true;

                _recentPurchased.Clear();
            });

            Task.Run(async () =>
            {
                return await CustomerWebService.Instance.GetRecentlyPurchasedItemsAsync(_rootId, _companyCode);

            }).ContinueWith(async (t) =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        if (t.IsCompleted) _recentPurchased.Set(t.Result);
                        RaisePropertyChanged("IsEmpty");
                    }
                    finally
                    {
                        Loading = false;
                    }
                });
            });
        }

        /// <summary>
        /// Sets the current pivot index.
        /// </summary>
        /// <param name="index">The new pivot index being maintained by the page.</param>
        public async void SetPivotIndex(int index)
        {
            try
            {
                var active = ((_index != PivotIndex) && (index == PivotIndex));
                var wasActive = ((_index == PivotIndex) && (index != PivotIndex));

                _index = index;

                if (active)
                {
                    await Load();
                }
                else if (wasActive)
                {
                    await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Loading = false; });
                }
            }
            finally
            {
            }
        }


        #endregion

        #region Public properties


        /// <summary>
        /// Collection of document groups.
        /// </summary>
        public ObservableCollectionEx<RecentPurchasedItem> RecentPurchasedItems
        {
            get { return _recentPurchased; }
        }

        /// <summary>
        /// Determines if the view is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return (_recentPurchased.Count == 0); }
        }

        /// <summary>
        /// True if loading.
        /// </summary>
        public bool Loading
        {
            get { return _loading; }
            set
            {
                if (_loading != value)
                {
                    _loading = value;
                    _owner.Loading = value;

                    RaisePropertyChanged("Loading");
                }
            }
        }

        #endregion
    }
}

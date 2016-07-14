/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.Sage100Services;
using Sage100AddressBook.Views;
using System;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

#pragma warning disable 4014

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// View model for the orders pivot item.
    /// </summary>
    public class OrderPivotViewModel : OrderSummaryViewModel
    {
        #region Private constants

        private const int PivotIndex = 3;

        #endregion

        #region Private fields

        private ObservableCollectionEx<OrderSummary> _orders = new ObservableCollectionEx<OrderSummary>();
        private DelegateCommand _refresh;
        private int _index = (-1);
        private bool _loading;
        private bool _loaded;
        private bool _isLoading;

        #endregion

        #region Private methods

        /// <summary>
        /// Performs the refresh on quotes.
        /// </summary>
        private async void RefreshAction()
        {
            if (_isLoading) return;

            _loaded = false;
            GlobalCache.OrderCache.Clear();

            await Load();
        }

        /// <summary>
        /// Attempts to reload the data from cache.
        /// </summary>
        /// <returns>True if loaded from cache, otherwise false.</returns>
        private bool LoadFromCache()
        {
            var orders = GlobalCache.OrderCache.Get(CompanyCode, RootId);

            if (orders == null) return false;

            foreach (var entry in orders) entry.Edit = Edit;

            _orders.Set(orders);

            RaisePropertyChanged("IsEmpty");

            return true;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public OrderPivotViewModel(ViewModelLoading owner) : base(owner, OrderType.Order)
        {
            _refresh = new DelegateCommand(new Action(RefreshAction));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads the documents from the retrieval service.
        /// </summary>
        /// <returns>The async task to wait on.</returns>
        public async Task Load()
        {
            if ((_index != PivotIndex) || _loaded || _isLoading) return;

            _isLoading = true;

            if (LoadFromCache())
            {
                _loaded = true;
                _isLoading = false;

                return;
            }

            var dispatcher = Window.Current.Dispatcher;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Loading = true;
                _orders.Clear();
            });

            Task.Run(async () =>
            {
                var orders = await CustomerWebService.Instance.GetOrdersSummaryAsync(RootId, CompanyCode);

                GlobalCache.OrderCache.Set(CompanyCode, RootId, orders);

                return orders;

            }).ContinueWith(async (t) =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        if (t.IsCompleted)
                        {
                            foreach (var entry in t.Result) entry.Edit = Edit;

                            _orders.Set(t.Result);
                            _loaded = true;
                        }

                        RaisePropertyChanged("IsEmpty");

                    }
                    finally
                    {
                        Loading = _isLoading = false;
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
                    if (_isLoading)
                    {
                        Loading = true;
                        return;
                    }

                    await Load();
                }
                else if (wasActive)
                {
                    await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Loading = false; });
                }
            }
            finally
            {
                UpdateState(_orders);
                RaisePropertyChanged("OrderCommandsVisible");
            }
        }

        /// <summary>
        /// Event that is triggered when the selected quote changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void OrderSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var grid = (sender as GridView);

                Current = grid.SelectedItem as OrderSummary;
                RaisePropertyChanged("OrderCommandsVisible");

                if ((Current != null) && (grid != null)) grid.ScrollIntoView(Current);
            }
            finally
            {
                /* Raise notification changes */
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Collection of document groups.
        /// </summary>
        public ObservableCollectionEx<OrderSummary> Orders
        {
            get { return _orders; }
        }

        /// <summary>
        /// Delegate command action for the refresh.
        /// </summary>
        public DelegateCommand Refresh
        {
            get { return _refresh; }
        }

        /// <summary>
        /// Determines if the view is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return (_orders.Count == 0); }
        }

        /// <summary>
        /// Determines if the order commands are available.
        /// </summary>
        public bool OrderCommandsVisible
        {
            get { return (_index == PivotIndex); }
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
                    Owner.Loading = value;

                    RaisePropertyChanged("Loading");
                }
            }
        }

        #endregion
    }
}
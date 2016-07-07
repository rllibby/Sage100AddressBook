/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.Sage100Services;
using System;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#pragma warning disable 4014

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// View model for the orders pivot item.
    /// </summary>
    public class OrderPivotViewModel : BindableBase
    {
        #region Private constants

        private const int PivotIndex = 3;

        #endregion

        #region Private fields

        private ObservableCollectionEx<OrderSummary> _orders = new ObservableCollectionEx<OrderSummary>();
        private ViewModelLoading _owner;
        private OrderSummary _current;
        private DelegateCommand _refresh;
        private string _companyCode;
        private string _rootId;
        private int _index = (-1);
        private bool _loading;
        private bool _loaded;
        private bool _isLoading;

        #endregion

        #region Private methods

        /// <summary>
        /// Determines if we have a current order.
        /// </summary>
        /// <returns>True if the quote is not null.</returns>
        private bool HasOrder(OrderSummary entry)
        {
            return ((entry != null) || (_current != null));
        }

        /// <summary>
        /// Performs the refresh on quotes.
        /// </summary>
        private async void RefreshAction()
        {
            if (_isLoading) return;

            _loaded = false;

            await Load();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public OrderPivotViewModel(ViewModelLoading owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
            _refresh = new DelegateCommand(new Action(RefreshAction));
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
            _companyCode = companyCode.ToLower();
        }

        /// <summary>
        /// Loads the documents from the retrieval service.
        /// </summary>
        /// <returns>The async task to wait on.</returns>
        public async Task Load()
        {
            if ((_index != PivotIndex) || _loaded || _isLoading) return;

            _isLoading = true;

            var dispatcher = Window.Current.Dispatcher;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Loading = true;
                _orders.Clear();
            });

            Task.Run(async () =>
            {
                return await CustomerWebService.Instance.GetOrdersSummaryAsync(_rootId, _companyCode);

            }).ContinueWith(async (t) =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        if (t.IsCompleted)
                        {
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
                Current = (sender as GridView)?.SelectedItem as OrderSummary;

                RaisePropertyChanged("OrderCommandsVisible");
            }
            finally
            {
                /* Raise notification changes */
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the current order summary entry.
        /// </summary>
        public OrderSummary Current
        {
            get { return _current; }
            set { Set(ref _current, value); }
        }

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
                    _owner.Loading = value;

                    RaisePropertyChanged("Loading");
                }
            }
        }

        #endregion
    }
}

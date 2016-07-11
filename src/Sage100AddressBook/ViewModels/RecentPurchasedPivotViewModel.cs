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
    /// View model for the recently purchased items pivot.
    /// </summary>
    public class RecentPurchasedPivotViewModel : BindableBase
    {
        #region Private constants

        private const int PivotIndex = 4;

        #endregion

        #region Private fields

        private ObservableCollectionEx<RecentPurchasedItem> _items = new ObservableCollectionEx<RecentPurchasedItem>();
        private ViewModelLoading _owner;
        private RecentPurchasedItem _current;
        private DelegateCommand _refresh;
        private string _companyCode;
        private string _rootId;
        private int _index = (-1);
        private bool _loading;
        private bool _isLoading;
        private bool _loaded;

        #endregion

        #region Private methods

        /// <summary>
        /// Determines if we have a current item.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns>True if the item is not null.</returns>
        private bool HasItem(RecentPurchasedItem item)
        {
            return ((item != null) || (_current != null));
        }

        /// <summary>
        /// Performs the refresh on recently purchased items.
        /// </summary>
        private async void RefreshAction()
        {
            if (_isLoading) return;

            _loaded = false;
            GlobalCache.RecentCache.Clear();

            await Load();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public RecentPurchasedPivotViewModel(ViewModelLoading owner)
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
            _companyCode = companyCode;
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

                _items.Clear();
            });

            Task.Run(async () =>
            {
                var recent = GlobalCache.RecentCache.Get(_companyCode, _rootId);

                if (recent != null) return recent;

                recent = await CustomerWebService.Instance.GetRecentlyPurchasedItemsAsync(_rootId, _companyCode);

                GlobalCache.RecentCache.Set(_companyCode, _rootId, recent);

                return recent;

            }).ContinueWith(async (t) =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        if (t.IsCompleted)
                        {
                            _items.Set(t.Result);
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
                RaisePropertyChanged("RecentItemCommandsVisible");
            }
        }

        /// <summary>
        /// Event that is triggered when the selected item changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void RecentItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Current = (sender as GridView)?.SelectedItem as RecentPurchasedItem;
            }
            finally
            {
                RaisePropertyChanged("RecentItemCommandsVisible");
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the current recent item entry.
        /// </summary>
        public RecentPurchasedItem Current
        {
            get { return _current; }
            set { Set(ref _current, value); }
        }

        /// <summary>
        /// Collection of document groups.
        /// </summary>
        public ObservableCollectionEx<RecentPurchasedItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// The action for refreshing the recent item list.
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
            get { return (_items.Count == 0); }
        }

        /// <summary>
        /// Determines if the recent item commands are available.
        /// </summary>
        public bool RecentItemCommandsVisible
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

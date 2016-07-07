/*
 *  Copyright © 2016, Sage Software, Inc.
 */

using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.CustomerSearchServices;
using Sage100AddressBook.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

#pragma warning disable 4014

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// View model for the search results page.
    /// </summary>
    public class SearchResultsPageViewModel : ViewModelBase
    {
        #region Private fields

        private ObservableCollectionEx<AddressGroup> _addressGroups = new ObservableCollectionEx<AddressGroup>();
        private List<AddressEntry> _addresses = new List<AddressEntry>();
        private CustomerSearchService _searchService;
        private AddressEntry _currentAddress;
        private string _search;
        private bool _loading;

        #endregion

        #region Private methods

        /// <summary>
        /// Executes the search on a background thread.
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        private void LoadSearchResults(string searchText)
        {
            var dispatcher = Window.Current.Dispatcher;

            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Loading = true; });

            Task.Run(async () =>
            {
                _addresses.Clear();

                return await _searchService.ExecuteSearchAsync(searchText);
            }).ContinueWith(async (t) =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        if (t.IsFaulted)
                        {
                            if (t.Exception != null) await Dialogs.ShowException(string.Format("Failed to search the documents for '{0}'.", searchText), t.Exception, false);
                        }

                        if (t.IsCompleted) _addresses.AddRange(t.Result);

                        BuildAddressGroups();
                    }
                    finally
                    {
                        Loading = false;
                    }
                });
            });
        }

        /// <summary>
        /// Build the address group data.
        /// </summary>
        private void BuildAddressGroups()
        {
            var grouped = from address in _addresses group address by address.Type into grp orderby grp.Key descending select new AddressGroup(grp.Key, grp.ToList());

            _addressGroups.Set(new ObservableCollection<AddressGroup>(grouped.ToList()));

            RaisePropertyChanged("IsEmpty");
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SearchResultsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) { }

            _searchService = new CustomerSearchService();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Called when the page is being naviagted to.
        /// </summary>
        /// <param name="parameter">The parameter passed during navigation.</param>
        /// <param name="mode">The navigation mode.</param>
        /// <param name="suspensionState">The saved state.</param>
        /// <returns>The async task to wait on.</returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Search = (suspensionState.ContainsKey(nameof(Search))) ? suspensionState[nameof(Search)]?.ToString() : parameter?.ToString();

            try
            {
                LoadSearchResults(Search);
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// Called when this view model is navigated from.
        /// </summary>
        /// <param name="suspensionState">The dictionary of application state.</param>
        /// <param name="suspending">True if application is suspending.</param>
        /// <returns>The async task.</returns>
        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            try
            {
                if (suspending) suspensionState[nameof(Search)] = Search;
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// Called when this view model is navigated from.
        /// </summary>
        /// <param name="args">The navigation event argument.</param>
        /// <returns>The async task.</returns>
        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            try
            {
                args.Cancel = false;
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// Event handler for the address click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The item click event argument.</param>
        public void AddressClicked(object sender, ItemClickEventArgs args)
        {
            _currentAddress = args.ClickedItem as AddressEntry;

            if (_currentAddress == null) return;

            RecentAddress.Instance.AddRecent(_currentAddress);

            NavigationService.Navigate(typeof(CustomerDetailPage), AddressEntry.GetNavArgs(_currentAddress), new SuppressNavigationTransitionInfo());
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Search value.
        /// </summary>
        public string Search
        {
            get { return _search; }
            set
            {
                Set(ref _search, value);

                RaisePropertyChanged("Results");
            }
        }
        
        /// <summary>
        /// Collection of address groups.
        /// </summary>
        public ObservableCollection<AddressGroup> AddressGroups
        {
            get { return _addressGroups; }
        }

        /// <summary>
        /// Current address.
        /// </summary>
        public AddressEntry CurrentAddress
        {
            get { return _currentAddress; }
            set { Set(ref _currentAddress, value); }
        }

        /// <summary>
        /// The results message.
        /// </summary>
        public string Results
        {
            get { return string.IsNullOrEmpty(_search)? "Search results" : string.Format("Search results for \"{0}\"", _search); }
        }

        /// <summary>
        /// Determines if the search list is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return (_addresses.Count == 0); }
        }

        /// <summary>
        /// True if loading, otherwise false.
        /// </summary>
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;

                base.RaisePropertyChanged();
            }
        }

        #endregion
    }
}
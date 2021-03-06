/*
 *  Copyright � 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.CustomControls;
using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// View model for the home page.
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        #region Private fields

        private FavoriteAddress _favoriteAddress = FavoriteAddress.Instance;
        private DelegateCommand<SearchControl> _search;
        private SearchControl _searchControl;

        #endregion

        #region Private methods

        /// <summary>
        /// Callback for search execution.
        /// </summary>
        /// <param name="sender">The sender of the event, which is the search control.</param>
        /// <param name="arg">The search event arguments.</param>
        private async void OnSearchResults(object sender, SearchEventArgs arg)
        {
            if (string.IsNullOrEmpty(arg.SearchText)) return;

            var search = arg.SearchText;
            var dispatcher = Dispatcher;

            await dispatcher.DispatchAsync(async () =>
            {
                try
                {
                    _searchControl?.Reset();
                    NavigationService.Navigate(typeof(Views.SearchResultsPage), search, new SuppressNavigationTransitionInfo());
                }
                catch (Exception exception)
                {
                    await Dialogs.ShowException(string.Format("Failed to search the documents for '{0}'.", search), exception, false);
                }
            });
        }

        /// <summary>
        /// Show the search control.
        /// </summary>
        private async void ShowSearch(SearchControl arg)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _searchControl = arg;
                _searchControl.AutoDismiss = false;
                _searchControl?.ShowSearch(OnSearchResults, "Enter a search string, e.g. name, number or city.");
            });
        }

        /// <summary>
        /// Action to close search results.
        /// </summary>
        /// <param name="arg">The search control.</param>
        private void CloseSearchAction(SearchControl arg)
        {
            /* No-op */
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) { }

            _search = new DelegateCommand<SearchControl>(new Action<SearchControl>(ShowSearch));
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
            try
            {
                if (_searchControl != null) _searchControl.Focus(FocusState.Programmatic);
                if (suspensionState.Any()) { }
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
            var address = args.ClickedItem as AddressEntry;

            if (address == null) return;

            RecentAddress.Instance.AddRecent(address);

            if (address.Type.Equals("customer", StringComparison.OrdinalIgnoreCase))
            {
                NavigationService.Navigate(typeof(CustomerDetailPage), AddressEntry.GetNavArgs(address), new SuppressNavigationTransitionInfo());
                return;
            }
        }

        /// <summary>
        /// Navigate to the settings page, pivot index 0.
        /// </summary>
        public void GotoSettings() => NavigationService.Navigate(typeof(Views.SettingsPage), 0, new SuppressNavigationTransitionInfo());

        /// <summary>
        /// Navigate to the settings page, pivot index 1.
        /// </summary>
        public void GotoAbout() => NavigationService.Navigate(typeof(Views.SettingsPage), 1, new SuppressNavigationTransitionInfo());

        #endregion

        #region Public properties

        /// <summary>
        /// Collection of address groups.
        /// </summary>
        public FavoriteAddress Favorites
        {
            get { return _favoriteAddress; }
        }

        /// <summary>
        /// The command handler for invoking search.
        /// </summary>
        public DelegateCommand<SearchControl> Search
        {
            get { return _search; }
        }

        /// <summary>
        /// Determines if the close search button should be visible.
        /// </summary>
        public bool CloseSearchVisible
        {
            get { return (_searchControl != null); }
        }

        #endregion
    }
}


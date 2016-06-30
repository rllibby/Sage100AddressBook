/*
 *  Copyright © 2016, Sage Software, Inc.
 */

using Sage100AddressBook.Models;
using Sage100AddressBook.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// View model for the favorites page.
    /// </summary>
    public class FavoritesPageViewModel : ViewModelBase
    {
        #region Private fields

        private FavoriteAddress _favoriteAddress = FavoriteAddress.Instance;
        private bool _loading;

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the navigation event arguments.
        /// </summary>
        /// <param name="entry">The address entry.</param>
        /// <returns>The navigation event args.</returns>
        private NavigationArgs GetNavArgs(AddressEntry entry)
        {
            if (entry == null) throw new ArgumentNullException("entry");

            return new NavigationArgs()
            {
                Id = (entry.Type == "contact") ? entry.ParentId : entry.Id,
                CompanyCode = "ABC",
                RemovePage = null
            };
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public FavoritesPageViewModel()
        {
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
            Loading = true;

            try
            {
            }
            finally
            {
                Loading = false;
            }

            await Task.CompletedTask;
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
                NavigationService.Navigate(typeof(CustomerDetailPage), GetNavArgs(address), new SuppressNavigationTransitionInfo());
                return;
            }
        }

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

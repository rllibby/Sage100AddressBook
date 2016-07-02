/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.Sage100Services;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Sage100AddressBook.Helpers;
using Windows.System;
using System.Text;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;

namespace Sage100AddressBook.ViewModels
{
    public class CustomerDetailPageViewModel : ViewModelBase
    {
        #region Private constants

        private const int PivotIndex = 0;

        #endregion

        #region Private fields

        private ObservableCollectionEx<OrderSummary> _quotes = new ObservableCollectionEx<OrderSummary>();
        private ObservableCollectionEx<OrderSummary> _orders = new ObservableCollectionEx<OrderSummary>();
        private ObservableCollectionEx<RecentPurchasedItem> _recentItems = new ObservableCollectionEx<RecentPurchasedItem>();
        private DocumentPivotViewModel _documentModel;
        private CustomerWebService _webService;
        private Customer _currentCustomer;
        private AddressEntry _customerAddress;
        private DelegateCommand _showMap;
        private DelegateCommand<FrameworkElement> _contact;
        private DelegateCommand _toggleFavorites;
        private string _id = string.Empty;
        private int _index;
        private int _loading;

        #endregion

        #region Private methods

        /// <summary>
        /// Fired when the pivot index changes.SetPivotIndex(_index);
        /// </summary>
        /// <param name="index"></param>
        private void SetPivotIndex(int index)
        {
            try
            {

            }
            finally
            {
                RaisePropertyChanged("GlanceCommandsVisible");
            }
        }

        /// <summary>
        /// Builds the chart data for the current customer.
        /// </summary>
        /// <param name="currentCustomer">The current customer.</param>
        private void BuildChartData(Customer currentCustomer)
        {
            AgingChartData = new ObservableCollection<PieChartData>();

            if (currentCustomer.CurrentBalance != 0)
            {
                AgingChartData.Add(new PieChartData() { Value = currentCustomer.CurrentBalance, Label = currentCustomer.CaptionCurrrent + "\n(" + currentCustomer.CurrentBalance.ToString("C") + ")" });
            }
            if (currentCustomer.AgingCategory1 != 0)
            {
                AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory1, Label = currentCustomer.CaptionAging1 + "\n(" + currentCustomer.AgingCategory1.ToString("C") + ")" });
            }
            if (currentCustomer.AgingCategory2 != 0)
            {
                AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory2, Label = currentCustomer.CaptionAging2 + "\n(" + currentCustomer.AgingCategory2.ToString("C") + ")" });
            }
            if (currentCustomer.AgingCategory3 != 0)
            {
                AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory3, Label = currentCustomer.CaptionAging3 + "\n(" + currentCustomer.AgingCategory3.ToString("C") + ")" });
            }
            if (currentCustomer.AgingCategory4 != 0)
            {
                AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory4, Label = currentCustomer.CaptionAging4 + "\n(" + currentCustomer.AgingCategory4.ToString("C") + ")" });
            }
        }

        /// <summary>
        /// Gets the address in a format suitable for a map query.
        /// </summary>
        /// <returns>The map address.</returns>
        private string GetAddressQuery()
        {
            var query = string.Empty;

            if (_currentCustomer == null) return null;

            if (!string.IsNullOrEmpty(_currentCustomer.AddressLine1)) query += _currentCustomer.AddressLine1;
            if (!string.IsNullOrEmpty(_currentCustomer.City)) query += string.Format("{0}{1}", string.IsNullOrEmpty(query) ? "" : ", ", _currentCustomer.City);
            if (!string.IsNullOrEmpty(_currentCustomer.State)) query += string.Format("{0}{1}", string.IsNullOrEmpty(query) ? "" : ", ", _currentCustomer.State);
            if (!string.IsNullOrEmpty(_currentCustomer.ZipCode)) query += string.Format("{0}{1}", string.IsNullOrEmpty(query) ? "" : ", ", _currentCustomer.ZipCode);

            return query.Trim();
        }

        /// <summary>
        /// Shows the customer address mapped in the map application.
        /// </summary>
        private async void ShowMapAction()
        {
            var mapUri = string.Format("bingmaps:?where={0}", Uri.EscapeUriString(GetAddressQuery()));
            var launcherOptions = new LauncherOptions();

            launcherOptions.TargetApplicationPackageFamilyName = "Microsoft.WindowsMaps_8wekyb3d8bbwe";

            await Launcher.LaunchUriAsync(new Uri(mapUri), launcherOptions);
        }

        /// <summary>
        /// Determines if the show map should be enabled.
        /// </summary>
        /// <returns></returns>
        private bool CanShowMap()
        {
            return (!string.IsNullOrEmpty(GetAddressQuery()));
        }

        /// <summary>
        /// Toggles the favorites for the current customer.
        /// </summary>
        private void ToggleFavoritesAction()
        {
            if (_customerAddress == null) return;

            try
            {
                FavoriteAddress.Instance.ToggleFavorite(_customerAddress);
            }
            finally
            {
                RaisePropertyChanged("FavoriteSymbol");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Contact CreateContactFromCustomer()
        {
            var number = new StringBuilder();
            var email = _currentCustomer.EmailAddress;

            if (!string.IsNullOrEmpty(_currentCustomer.Telephone))
            {
                foreach (var c in _currentCustomer.Telephone)
                {
                    if (char.IsNumber(c)) number.Append(c);
                }
            }

            var contact = new Contact();
            var address = new ContactAddress();

            address.Country = "USA";
            address.Description = "Office";
            address.Kind = ContactAddressKind.Work;
            address.Locality = _currentCustomer.City;
            address.Region = _currentCustomer.State;
            address.PostalCode = _currentCustomer.ZipCode;
            address.StreetAddress = _currentCustomer.AddressLine1;

            contact.Addresses.Add(address);

            if (!string.IsNullOrEmpty(email))
            {
                contact.Emails.Add(new ContactEmail { Address = email });
            }

            if (!string.IsNullOrEmpty(number.ToString()))
            {
                contact.Phones.Add(new ContactPhone { Number = number.ToString() });
            }

            return contact;
        }

        /// <summary>
        /// Shows the contact card for the customer.
        /// </summary>
        private void ContactAction(FrameworkElement sender)
        {
            if (_currentCustomer == null) return;

            var contact = CreateContactFromCustomer();
            var transform = sender.TransformToVisual(null);
            var point = transform.TransformPoint(new Point());
            var rect = new Rect(point, new Size(sender.ActualWidth, sender.ActualHeight));

            if (Device.IsMobile)
            {
                ContactManager.ShowFullContactCard(contact, new FullContactCardOptions { DesiredRemainingView = ViewSizePreference.Default });
                return;
            }

            ContactManager.ShowContactCard(contact, rect, Placement.Below);
        }

        /// <summary>
        /// Determines if the show contact action is available.
        /// </summary>
        /// <returns>True if we have a customer.</returns>
        private bool CanShowContact(FrameworkElement sender)
        {
            return ((_currentCustomer != null) && (!string.IsNullOrEmpty(_currentCustomer.Telephone) || !string.IsNullOrEmpty(_currentCustomer.EmailAddress)));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public CustomerDetailPageViewModel()
        {
            _webService = new CustomerWebService();
            _documentModel = new DocumentPivotViewModel(this);
            _toggleFavorites = new DelegateCommand(new Action(ToggleFavoritesAction));
            _showMap = new DelegateCommand(new Action(ShowMapAction), CanShowMap);
            _contact = new DelegateCommand<FrameworkElement>(new Action<FrameworkElement>(ContactAction), CanShowContact);
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
                var navArgs = (NavigationArgs)parameter;
                var index = (suspensionState.ContainsKey("Index")) ? suspensionState["Index"]?.ToString() : "0";

                Index = (!string.IsNullOrEmpty(index) ? Convert.ToInt32(index) : 0);
                CurrentCustomer = await _webService.GetCustomerAsync(navArgs.Id, navArgs.CompanyCode);

                _id = CurrentCustomer.Id;
                _documentModel.SetPivotIndex(Index);
                _documentModel.SetArguments(navArgs.Id, navArgs.CompanyCode);
                _customerAddress = CurrentCustomer.GetAddressEntry();

                BuildChartData(CurrentCustomer);

                _quotes.Set(await _webService.GetQuotesSummaryAsync(navArgs.Id, navArgs.CompanyCode),Dispatcher);
                _orders.Set(await _webService.GetOrdersSummaryAsync(navArgs.Id, navArgs.CompanyCode), Dispatcher);
                _recentItems.Set(await _webService.GetRecentlyPurchasedItemsAsync(navArgs.Id, navArgs.CompanyCode), Dispatcher);
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
                suspensionState["Index"] = _index;
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// Maintains the currently selected pivot item.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void PivotChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = sender as Pivot;

            if (pivot != null) _index = pivot.SelectedIndex;

            SetPivotIndex(_index);
            _documentModel.SetPivotIndex(_index);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The model handling the document pivot page.
        /// </summary>
        public DocumentPivotViewModel DocumentModel
        {
            get { return _documentModel; }
        }

        /// <summary>
        /// Show the map with customer location.
        /// </summary>
        public DelegateCommand ShowMap
        {
            get { return _showMap; }
        }

        /// <summary>
        /// The current customer.
        /// </summary>
        public Customer CurrentCustomer
        {
            get { return _currentCustomer; }
            set { Set(ref _currentCustomer, value); }
        }

        /// <summary>
        /// The collection of chart data.
        /// </summary>
        public ObservableCollection<PieChartData> AgingChartData { get; private set; }

        /// <summary>
        /// Determines if glance commands should be visible.
        /// </summary>
        public bool GlanceCommandsVisible
        {
            get { return (_index == PivotIndex); }
        }

        /// <summary>
        /// Returns the favorite symbol based on favorite state.
        /// </summary>
        public string FavoriteSymbol
        {
            get { return FavoriteAddress.Instance.IsFavorite(_id) ? "SolidStar" : "OutlineStar"; }
        }

        /// <summary>
        /// Command for toggling the favorite state.
        /// </summary>
        public DelegateCommand ToggleFavorite
        {
            get { return _toggleFavorites; }
        }

        /// <summary>
        /// Command for showing the contact card.
        /// </summary>
        public DelegateCommand<FrameworkElement> Contact
        {
            get { return _contact; }
        }

        /// <summary>
        /// The current pivot index.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { Set(ref _index, value); }
        }

        /// <summary>
        /// True if loading, otherwise false.
        /// </summary>
        public bool Loading
        {
            get { return (_loading > 0); }
            set
            {
                _loading += (value ? 1 : (-1));
                _loading = Math.Max(0, _loading);
                 
                base.RaisePropertyChanged();
            }
        }

        #endregion
    }
}

/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.Sage100Services;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using System;

namespace Sage100AddressBook.ViewModels
{
    public class CustomerDetailPageViewModel : ViewModelBase
    {
        #region Private constants

        private const int PivotIndex = 0;

        #endregion

        #region Private fields

        private DocumentPivotViewModel _documentModel;
        private CustomerWebService _webService;
        private Customer _currentCustomer;
        private AddressEntry _customerAddress;
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
                AgingChartData.Add(new PieChartData() { Value = currentCustomer.CurrentBalance, Label = currentCustomer.CaptionCurrrent + " (" + currentCustomer.CurrentBalance.ToString() + ")" });
            }
            if (currentCustomer.AgingCategory1 != 0)
            {
                AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory1, Label = currentCustomer.CaptionAging1 + " (" + currentCustomer.AgingCategory1.ToString() + ")" });
            }
            if (currentCustomer.AgingCategory2 != 0)
            {
                AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory2, Label = currentCustomer.CaptionAging2 + " (" + currentCustomer.AgingCategory2.ToString() + ")" });
            }
            if (currentCustomer.AgingCategory3 != 0)
            {
                AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory3, Label = currentCustomer.CaptionAging3 + " (" + currentCustomer.AgingCategory3.ToString() + ")" });
            }
            if (currentCustomer.AgingCategory4 != 0)
            {
                AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory4, Label = currentCustomer.CaptionAging4 + " (" + currentCustomer.AgingCategory4.ToString() + ")" });
            }
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

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public CustomerDetailPageViewModel()
        {
            _webService = new CustomerWebService();
            _currentCustomer = new Customer();
            _documentModel = new DocumentPivotViewModel(this);
            _toggleFavorites = new DelegateCommand(new Action(ToggleFavoritesAction));
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

                CurrentCustomer = await _webService.GetCustomerAsync(navArgs.Id, navArgs.CompanyCode);

                _id = CurrentCustomer.Id;
                _documentModel.SetPivotIndex(0);
                _documentModel.SetArguments(navArgs.Id, navArgs.CompanyCode);
                _customerAddress = CurrentCustomer.GetAddressEntry();

                BuildChartData(CurrentCustomer);
            }
            finally
            {
                Loading = false;
            }

            await Task.CompletedTask;
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

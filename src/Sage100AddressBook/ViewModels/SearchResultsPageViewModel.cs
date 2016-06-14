using Sage100AddressBook.Models;
using Sage100AddressBook.Services.CustomerSearchServices;
using Sage100AddressBook.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.ViewModels
{
    public class SearchResultsPageViewModel : ViewModelBase
    {

        //private readonly IDataModelService _dataModelService = DataModelServiceFactory.CurrentDataModelService();
        private AddressEntry _currentAddress;

        private CustomerSearchService _searchService;



        private ObservableCollection<AddressGroup> _addressGroups;
        //private BitmapImage _sightImage;
        private ObservableCollection<AddressEntry> _addresses;

        public SearchResultsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Search = "Designtime value";
            }
            _searchService = new CustomerSearchService();
        }

        private string _Search = "Default";
        public string Search { get { return _Search; } set { Set(ref _Search, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Search = (suspensionState.ContainsKey(nameof(Search))) ? suspensionState[nameof(Search)]?.ToString() : parameter?.ToString();

            _addresses = new ObservableCollection<AddressEntry>(await _searchService.ExecuteSearchAsync(Search));

            if (_addresses.Count == -1)  //to-do this works just need to figure out how to handle back button to return to search
            {
                NavigationService.Navigate(typeof(CustomerDetailPage), GetNavArgs(_addresses.FirstOrDefault()));
                //NavigationService.ClearCache(true);
            }
            else
            {
                BuildAddressGroups();
            }
            
            await Task.CompletedTask;
        }

        private void BuildAddressGroups()
        {
            var grouped = from address in Addresses
                            group address by address.Type
                into grp
                            orderby grp.Key descending
                            select new AddressGroup
                            {
                                GroupName = grp.Key + "s",
                                AddressEntries = grp.ToList()
                            };

            AddressGroups = new ObservableCollection<AddressGroup>(grouped.ToList());
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Search)] = Search;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }


        public ObservableCollection<AddressGroup> AddressGroups
        {
            get { return _addressGroups; }
            set { Set(ref _addressGroups, value); }
        }

        public ObservableCollection<AddressEntry> Addresses
        {
            get { return _addresses; }
            set { Set(ref _addresses, value); }
        }

        public AddressEntry CurrentAddress
        {
            get { return _currentAddress; }
            set { Set(ref _currentAddress, value); }
        }
        public void AddressClicked(object sender, ItemClickEventArgs args)
        {
            _currentAddress = args.ClickedItem as AddressEntry;


            //AppShell.Current.NavigateToPage(typeof(AddressDetailPage), CurrentSight.Id.ToString("D"));
            //NavigationService.Navigate(typeof(DocumentViewerPage));
            NavigationService.Navigate(typeof(CustomerDetailPage), GetNavArgs(_currentAddress));
        }

        private NavigationArgs GetNavArgs(AddressEntry entry)
        {
            string tempId = "";

            if (entry.Type == "contact")
            {
                tempId = entry.ParentId;
            }
            else
            {
                tempId = entry.Id;
            }
            var navArgs = new NavigationArgs()
            {

                Id = tempId,
                CompanyCode = "ABC" //to-do need to add company code to the model so it's there
            };

            return navArgs;
        }

    }
}
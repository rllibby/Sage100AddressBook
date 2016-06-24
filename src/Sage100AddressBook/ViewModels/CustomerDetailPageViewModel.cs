using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.Sage100Services;
using Sage100AddressBook.Services.DocumentViewerServices;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Sage100AddressBook.Views;
using Windows.UI.Xaml.Media.Animation;

namespace Sage100AddressBook.ViewModels
{
    public class CustomerDetailPageViewModel : ViewModelBase
    {
        #region Private fields

        //private readonly IDataModelService _dataModelService = DataModelServiceFactory.CurrentDataModelService();
        private Customer _currentCustomer;
        private CustomerWebService _webService;
        private DocumentEntry _currentDocument;
        private DocumentRetrievalService _docRetrievalService;
        private ObservableCollection<DocumentGroup> _documentGroups;
        //private BitmapImage _sightImage;
        private ObservableCollection<DocumentEntry> _documents;
        private bool _loading;

        #endregion


        public CustomerDetailPageViewModel()
        {
            _webService = new CustomerWebService();
            _currentCustomer = new Customer();
            _docRetrievalService = new DocumentRetrievalService();
        }
        public Customer CurrentCustomer
        {
            get { return _currentCustomer; }
            set { Set(ref _currentCustomer, value); }
        }


        public ObservableCollection<PieChartData> AgingChartData { get; private set; }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Loading = true;

            try
            {
                //Customer = (suspensionState.ContainsKey(nameof(Customer))) ? suspensionState[nameof(Customer)]?.ToString() : parameter?);
                var navArgs = (NavigationArgs)parameter;

                CurrentCustomer = await _webService.GetCustomerAsync(navArgs.Id, navArgs.CompanyCode);
                buildChartData(CurrentCustomer);
                //to-do make several additional calls for quotes, orders, invoices, etc. - but don't await??

                //grab folders and documents for this customer - to-do: perhaps do this when pivot page is opened OR in a task.run
                _documents = new ObservableCollection<DocumentEntry>(await _docRetrievalService.RetrieveDocumentsAsync(navArgs.Id, navArgs.CompanyCode));
                BuildDocumentGroups();
            }
            finally
            {
                Loading = false;
            }
            await Task.CompletedTask;
        }

        private void buildChartData(Customer currentCustomer)
        {
            {
                AgingChartData = new ObservableCollection<PieChartData>();
                if (currentCustomer.CurrentBalance != 0)
                {
                    AgingChartData.Add(new PieChartData() { Value = currentCustomer.CurrentBalance, Label = currentCustomer.CaptionCurrrent });
                }
                if (currentCustomer.AgingCategory1 != 0)
                {
                    AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory1, Label = currentCustomer.CaptionAging1 });
                }
                if (currentCustomer.AgingCategory2 != 0)
                {
                    AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory2, Label = currentCustomer.CaptionAging2 });
                }
                if (currentCustomer.AgingCategory3 !=0)
                {
                    AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory3, Label = currentCustomer.CaptionAging3 });
                }
                if (currentCustomer.AgingCategory4 !=0)
                {
                    AgingChartData.Add(new PieChartData() { Value = currentCustomer.AgingCategory4, Label = currentCustomer.CaptionAging4 });
                }
                
            }
        }

        private void BuildDocumentGroups()
        {
            var grouped = from document in Documents
                          group document by document.Folder
                into grp
                          orderby grp.Key descending
                          select new DocumentGroup
                          {
                              GroupName = grp.Key,
                              DocumentEntries = grp.ToList()
                          };

            DocumentGroups = new ObservableCollection<DocumentGroup>(grouped.ToList());
        }

        public ObservableCollection<DocumentGroup> DocumentGroups
        {
            get { return _documentGroups; }
            set { Set(ref _documentGroups, value); }
        }

        public ObservableCollection<DocumentEntry> Documents
        {
            get { return _documents; }
            set { Set(ref _documents, value); }
        }

        public DocumentEntry CurrentDocument
        {
            get { return _currentDocument; }
            set { Set(ref _currentDocument, value); }
        }
        public void DocumentClicked(object sender, ItemClickEventArgs args)
        {
            _currentDocument = args.ClickedItem as DocumentEntry;


            //AppShell.Current.NavigateToPage(typeof(AddressDetailPage), CurrentSight.Id.ToString("D"));
            //NavigationService.Navigate(typeof(DocumentViewerPage));
            NavigationService.Navigate(typeof(DocumentViewerPage), _currentDocument, new SuppressNavigationTransitionInfo());
        }

        #region Public properties

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

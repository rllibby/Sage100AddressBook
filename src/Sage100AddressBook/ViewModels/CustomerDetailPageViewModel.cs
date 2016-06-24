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
using Sage100AddressBook.Helpers;
using Windows.UI.Xaml.Input;

namespace Sage100AddressBook.ViewModels
{
    public class CustomerDetailPageViewModel : ViewModelBase
    {
        #region Private fields

        private ObservableCollectionEx<DocumentGroup> _documentGroups = new ObservableCollectionEx<DocumentGroup>();
        private ObservableCollectionEx<DocumentEntry> _documents = new ObservableCollectionEx<DocumentEntry>();
        private Customer _currentCustomer;
        private CustomerWebService _webService;
        private DocumentEntry _currentDocument;
        private DocumentRetrievalService _docRetrievalService;
        private DelegateCommand _open;
        private int _pivotIndex;
        private bool _loading;

        #endregion

        #region Private methods

        /// <summary>
        /// Opens the document.
        /// </summary>
        /// <returns>The async task to wait on.</returns>
        private async Task DoDocumentOpen()
        {
            var document = CurrentDocument;
            var service = DocumentViewerService.Instance;

            if (document == null) return;

            Loading = true;

            var fileName = string.Empty;

            try
            {
                using (var stream = await service.GetFileStream(document.Id))
                {
                    fileName = await service.SaveToFile(stream, document.Name);
                    await service.LaunchFileAssociation(fileName);
                }
            }
            catch (Exception exception)
            {
                await Dialogs.ShowException(string.Format("Failed to launch the file '{0}'.", fileName), exception, false);
            }
            finally
            {
                Loading = false;
            }
        }

        /// <summary>
        /// Routes to the document open task.
        /// </summary>
        private async void OpenDocument()
        {
            await DoDocumentOpen();
        }

        /// <summary>
        /// Determines if we can open the document.
        /// </summary>
        /// <returns></returns>
        private bool CanOpenDocument()
        {
            return (CurrentDocument != null);
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
        /// Builds the document groups.
        /// </summary>
        private void BuildDocumentGroups()
        {
            var grouped = from document in Documents group document by document.Folder into grp orderby grp.Key descending select new DocumentGroup
                        {
                            GroupName = grp.Key,
                            DocumentEntries = grp.ToList()
                        };

            _documentGroups.Set(grouped.ToList());
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
            _docRetrievalService = new DocumentRetrievalService();
            _open = new DelegateCommand(new Action(OpenDocument), CanOpenDocument);
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
                BuildChartData(CurrentCustomer);
                _documents.Set(await _docRetrievalService.RetrieveDocumentsAsync(navArgs.Id, navArgs.CompanyCode));
                BuildDocumentGroups();
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

            if (pivot != null) _pivotIndex = pivot.SelectedIndex;

            RaisePropertyChanged("OpenVisible");
        }

        /// <summary>
        /// Event that is triggered when the selected document changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void DocumentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CurrentDocument = (sender as GridView)?.SelectedItem as DocumentEntry;
            }
            finally
            {
                _open.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Event that is triggered when the document is double tapped.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public async void DocumentDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (CurrentDocument == null) return;

            await DoDocumentOpen();
        }

        #endregion

        #region Public properties

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
        /// The collection of document groups.
        /// </summary>
        public ObservableCollection<DocumentGroup> DocumentGroups
        {
            get { return _documentGroups; }
        }

        /// <summary>
        /// The collection of documents.
        /// </summary>
        public ObservableCollection<DocumentEntry> Documents
        {
            get { return _documents; }
        }

        /// <summary>
        /// The command handler for the open button.
        /// </summary>
        public DelegateCommand Open
        {
            get { return _open; }
        }

        /// <summary>
        /// Determines if the open command is visible.
        /// </summary>
        public bool OpenVisible
        {
            get { return (_pivotIndex == 1); }
        }

        /// <summary>
        /// The currently selected document.
        /// </summary>
        public DocumentEntry CurrentDocument
        {
            get { return _currentDocument; }
            set { Set(ref _currentDocument, value); }
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

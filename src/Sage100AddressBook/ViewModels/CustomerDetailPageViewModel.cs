using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.Sage100Services;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.ViewModels
{
    public class CustomerDetailPageViewModel : ViewModelBase
    {
        //private readonly IDataModelService _dataModelService = DataModelServiceFactory.CurrentDataModelService();
        private Customer _currentCustomer;

        private CustomerWebService _webService;

        public CustomerDetailPageViewModel()
        {
            _webService = new CustomerWebService();
            _currentCustomer = new Customer();
        }
        public Customer CurrentCustomer
        {
            get { return _currentCustomer; }
            set { Set(ref _currentCustomer, value); }
        }


        public ObservableCollection<PieChartData> AgingChartData { get; private set; }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //Customer = (suspensionState.ContainsKey(nameof(Customer))) ? suspensionState[nameof(Customer)]?.ToString() : parameter?);
            var navArgs = (NavigationArgs)parameter;
            CurrentCustomer = await _webService.GetCustomerAsync(navArgs.Id, navArgs.CompanyCode);
            buildChartData(CurrentCustomer);
            //to-do make several additional calls for quotes, orders, invoices, etc. - but don't await??
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
    }
}

using Newtonsoft.Json;
using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private DelegateCommand _users;
        private ObservableCollection<GraphUser> _userList = new ObservableCollection<GraphUser>();

        private async void GetUsers()
        {
            if (!AuthenticationHelper.Instance.SignedIn) await AuthenticationHelper.Instance.SignIn();

            var data = await EndpointHelper.GetJson("users", AuthenticationHelper.Instance.Token);

            if (data != null)
            {
                var collection = JsonConvert.DeserializeObject<GraphUsers>(data);

                _userList.Clear();

                foreach (var user in collection.Users)
                {
                    _userList.Add(user);
                }
            }
        }

        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Search = "Designtime value";
            }

            _users = new DelegateCommand(new Action(GetUsers));
        }

        string _Value = "";
        public string Search { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Search = suspensionState[nameof(Search)]?.ToString();
            }
            await Task.CompletedTask;
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

        public void GotoDetailsPage() =>
            NavigationService.Navigate(typeof(Views.DetailPage), Search, new SuppressNavigationTransitionInfo());

        public void GotoSearchResultsPage() =>
            NavigationService.Navigate(typeof(Views.SearchResultsPage), Search, new SuppressNavigationTransitionInfo());

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0, new SuppressNavigationTransitionInfo());

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1, new SuppressNavigationTransitionInfo());

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2, new SuppressNavigationTransitionInfo());

        public DelegateCommand Users
        {
            get { return _users; }
        }

        public ObservableCollection<GraphUser> UserList
        {
            get { return _userList; }
        }

    }
}


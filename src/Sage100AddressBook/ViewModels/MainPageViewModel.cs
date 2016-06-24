/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
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
        private ObservableCollectionEx<object> _userList = new ObservableCollectionEx<object>();
        private DelegateCommand _users;

        private async void GetUsers()
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                var client = await AuthenticationHelper.GetClient();

                if (client == null) return;

                var users = await client.Users.Request().GetAsync();
                var steve = users[users.Count - 1];

                if (steve == null) return;

                // await client.Groups[group.Id].Owners.References.Request().AddAsync(me);
                // await client.Groups[group.Id].Members.References.Request().AddAsync(me);

                //var drive = await client.Me.Drive.Root.ItemWithPath("303141564E4554").Children.Request().GetAsync();
                //var drive = await client.Me.Drive.Root.Children.Request().GetAsync();

                // 01TOWMY6NRLXLIHDGJGJEYARBZ2IAJFBOG

                //var response = await client.Me.Drive.Items["01TOWMY6NRLXLIHDGJGJEYARBZ2IAJFBOG"].Content.Request().GetAsync();

                _userList.BeginUpdate();

                try
                {
                    _userList.Clear();
                    foreach (var user in users)
                    {
                        _userList.Add(user.DisplayName);
                    }
                }
                finally
                {
                    _userList.EndUpdate(Dispatcher);
                }
                
            });
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

        public ObservableCollection<object> UserList
        {
            get { return _userList; }
        }

    }
}


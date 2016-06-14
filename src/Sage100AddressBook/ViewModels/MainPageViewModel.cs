using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;

namespace Sage100AddressBook.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Search = "Designtime value";
            }
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

    }
}


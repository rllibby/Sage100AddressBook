/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// Shell view model.
    /// </summary>
    class ShellPageViewModel : ViewModelBase
    {
        #region Private properties

        private DelegateCommand _accounts;

        #endregion

        #region Private methods

        /// <summary>
        /// Performs the signin and signout for the account.
        /// </summary>
        private async void SignInOut()
        {
            if (await Dialogs.ShowOkCancel("Are you sure you wish to sign out?") == false) return;

            if (AuthenticationHelper.Instance.SignedIn)
            {
                AuthenticationHelper.Instance.SignOut();
                return;
            }

            await AuthenticationHelper.Instance.SignIn();
        }

        /// <summary>
        /// Determines if the account can signin or signout.
        /// </summary>
        /// <returns></returns>
        private bool CanSignInOut()
        {
            return true;
        }

        /// <summary>
        /// Event handler for authentication notification change.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The routed event arguments.</param>
        private void OnSignInChange(object sender, RoutedEventArgs args)
        {
            RaisePropertyChanged("UserName");
            RaisePropertyChanged("SignedIn");
            RaisePropertyChanged("SignInText");
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ShellPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) { }

            _accounts = new DelegateCommand(SignInOut, CanSignInOut);
            AuthenticationHelper.Instance.SignedInChanged += OnSignInChange;

        }

        #endregion

        #region Public methods

        /// <summary>
        /// Event that is triggered when this view model is navigated to.
        /// </summary>
        /// <param name="parameter">The sender of the event.</param>
        /// <param name="mode">The navigation mode.</param>
        /// <param name="suspensionState">The dictionary of application state.</param>
        /// <returns>The async task.</returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
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
        /// Event that is triggered when this view model is navigated from.
        /// </summary>
        /// <param name="suspensionState">The dictionary of application state.</param>
        /// <param name="suspending">True if application is suspending.</param>
        /// <returns>The async task.</returns>
        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Event that is triggered when this view model is about to be navigated from.
        /// </summary>
        /// <param name="args">The navigating event arguments.</param>
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

        #endregion

        #region Public properties

        /// <summary>
        /// The relay command for the accounts button.
        /// </summary>
        public DelegateCommand Accounts
        {
            get { return _accounts; }
        }

        /// <summary>
        /// The authenticated user.
        /// </summary>
        public string UserName
        {
            get { return AuthenticationHelper.Instance.UserName; }

        }

        /// <summary>
        /// Gets the text for the signin (out) button.
        /// </summary>
        public string SignInText
        {
            get { return (AuthenticationHelper.Instance.SignedIn ? "Sign out" : "Sign in"); }
        }

        /// <summary>
        /// True if authenticated, otherwise false.
        /// </summary>
        public bool SignedIn
        {
            get { return AuthenticationHelper.Instance.SignedIn; }
        }

        #endregion
    }
}

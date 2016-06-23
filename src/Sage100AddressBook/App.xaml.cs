/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using Sage100AddressBook.Services.SettingsServices;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Controls;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;
using Windows.ApplicationModel;
using System.IO.IsolatedStorage;

namespace Sage100AddressBook
{
    /// <summary>
    /// The application class.
    /// </summary>
    [Bindable]
    sealed partial class App : BootStrapper
    {
        #region Private methods

        /// <summary>
        /// Cleanup the temporary files in the user's isolated storage.
        /// </summary>
        private void CleanupIsolatedStorage()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                foreach (var file in store.GetFileNames())
                {
                    try
                    {
                        store.DeleteFile(file);
                    }
                    catch { }
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public App()
        {
            InitializeComponent();

            SplashFactory = (e) => new Views.Splash(e);

            var _settings = SettingsService.Instance;

            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Initialization.
        /// </summary>
        /// <param name="args">The activatino event arguments.</param>
        /// <returns>The async task.</returns>
        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (Window.Current.Content as ModalDialog == null)
            {
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }

            CleanupIsolatedStorage();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Application starting.
        /// </summary>
        /// <param name="startKind">The reason for starting; launching or activation.</param>
        /// <param name="args">The activation event arguments.</param>
        /// <returns>The async task.</returns>
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Black;
            titleBar.ButtonForegroundColor = Colors.White;

            try
            {
                await AuthenticationHelper.Instance.SignIn();

                NavigationService.Navigate(typeof(Views.MainPage), null, new SuppressNavigationTransitionInfo());
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// Method that is called when suspending or shutting down.
        /// </summary>
        /// <param name="s">The sender of the event.</param>
        /// <param name="e">Thesuspending event arguments.</param>
        /// <param name="prelaunchActivated">True if pre-launch activated.</param>
        /// <returns></returns>
        public override Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            try
            {
                CleanupIsolatedStorage();

                return base.OnSuspendingAsync(s, e, prelaunchActivated);
            }
            finally
            {
                deferral.Complete();
            }
        }

        #endregion
    }
}


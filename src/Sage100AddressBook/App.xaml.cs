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
using Windows.Foundation.Metadata;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Resources;
using Telerik.UI.Xaml.Controls;
using System;
using System.Reflection;
using Windows.UI.Xaml.Media;

namespace Sage100AddressBook
{
    /// <summary>
    /// The application class.
    /// </summary>
    [Bindable]
    sealed partial class App : BootStrapper
    {
        #region Private fields

        private static Rect _bounds;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public App()
        {
            InitializeComponent();

            SplashFactory = (e) => new Views.Splash(e);

            var settings = SettingsService.Instance;

            RequestedTheme = settings.AppTheme;
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;
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
            var view = ApplicationView.GetForCurrentView();

            _bounds = view.VisibleBounds;

            if (Device.IsMobile)
            {
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            }

            SettingsService.Instance.UpdateCardBrush();

            view.TitleBar.ButtonBackgroundColor = Colors.Black;
            view.TitleBar.ButtonForegroundColor = Colors.White;
            view.TitleBar.BackgroundColor = Colors.Black;
            view.TitleBar.ForegroundColor = Colors.White;
            view.TitleBar.ButtonHoverForegroundColor = Colors.DarkGray;
            view.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 0, 220, 0);
            view.TitleBar.ButtonPressedForegroundColor = Colors.White;
            view.TitleBar.ButtonInactiveBackgroundColor = Color.FromArgb(255, 40, 40, 40);
            view.TitleBar.ButtonInactiveForegroundColor = Colors.White;
            view.TitleBar.InactiveBackgroundColor = Color.FromArgb(255, 40, 40, 40);
            view.TitleBar.InactiveForegroundColor = Colors.White;

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                var statusBar = StatusBar.GetForCurrentView();

                statusBar.BackgroundColor = Colors.Black;
                statusBar.ForegroundColor = Colors.White;
                statusBar.BackgroundOpacity = 1;
            }

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
                 return base.OnSuspendingAsync(s, e, prelaunchActivated);
            }
            finally
            {
                deferral.Complete();
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the bounds of the display, which should only be used on mobile devices.
        /// </summary>
        public static Rect Bounds
        {
            get { return _bounds; }
        }

        #endregion
    }
}


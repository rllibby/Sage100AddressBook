/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace Sage100AddressBook.Services.SettingsServices
{
    /// <summary>
    /// Settings service.
    /// </summary>
    public class SettingsService
    {
        #region Private fields

        Template10.Services.SettingsService.ISettingsHelper _helper;
        private static SettingsService _instance = new SettingsService();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Determines if the shell back button is shown.
        /// </summary>
        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.Dispatcher.Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                    BootStrapper.Current.NavigationService.Refresh();
                });
            }
        }

        /// <summary>
        /// The application theme.
        /// </summary>
        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = Device.IsMobile ? ApplicationTheme.Dark : ApplicationTheme.Light;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());

                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                Views.Shell.HamburgerMenu.RefreshStyles(value);
            }
        }

        /// <summary>
        /// The max duration to holde cached pages.
        /// </summary>
        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        /// <summary>
        /// The singleton instance of the settings service.
        /// </summary>
        public static SettingsService Instance
        {
            get { return _instance; }
        }

        #endregion
    }
}


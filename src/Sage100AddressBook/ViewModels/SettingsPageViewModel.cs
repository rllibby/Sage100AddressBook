/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// View model for settings page.
    /// </summary>
    public class SettingsPageViewModel : ViewModelBase
    {
        #region Public properties

        /// <summary>
        /// Settings pivot view model.
        /// </summary>
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();

        /// <summary>
        /// About pivot view model
        /// </summary>
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();

        #endregion
    }

    /// <summary>
    /// Settings pivot view model
    /// </summary>
    public class SettingsPartViewModel : ViewModelBase
    {
        #region Private fields

        Services.SettingsServices.SettingsService _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsPartViewModel()
        {
            _settings = Services.SettingsServices.SettingsService.Instance;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Use the shell back button.
        /// </summary>
        public bool UseShellBackButton
        {
            get { return _settings.UseShellBackButton; }
            set { _settings.UseShellBackButton = value; base.RaisePropertyChanged(); }
        }

        /// <summary>
        /// Use light theme in application.
        /// </summary>
        public bool UseLightThemeButton
        {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set { _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; base.RaisePropertyChanged(); }
        }

        #endregion
    }

    /// <summary>
    /// About pivot view model.
    /// </summary>
    public class AboutPartViewModel : ViewModelBase
    {
        #region Public properties

        /// <summary>
        /// The logo image.
        /// </summary>
        public Uri Logo => new Uri("ms-appx:///Assets/Square44x44Logo.png");

        /// <summary>
        /// The display name.
        /// </summary>
        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        /// <summary>
        /// The publisher.
        /// </summary>
        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        /// <summary>
        /// The application version.
        /// </summary>
        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        #endregion
    }
}


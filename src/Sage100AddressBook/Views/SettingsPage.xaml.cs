/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.Views
{
    /// <summary>
    /// Code behind for the Settings page.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        #endregion
    }
}

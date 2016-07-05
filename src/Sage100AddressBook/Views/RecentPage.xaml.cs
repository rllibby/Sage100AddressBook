/*
 *  Copyright © 2016, Sage Software, Inc.
 */

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.Views
{
    /// <summary>
    /// Code behind for RecentPage.
    /// </summary>
    public sealed partial class RecentPage : Page
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public RecentPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        #endregion
    }
}

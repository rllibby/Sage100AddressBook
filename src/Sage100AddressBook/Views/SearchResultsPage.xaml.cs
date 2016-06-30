/*
 *  Copyright © 2016, Sage Software, Inc.
 */

using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.Views
{
    /// <summary>
    /// Code behind for the SearchResultsPage.
    /// </summary>
    public sealed partial class SearchResultsPage : Page
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SearchResultsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        #endregion
    }
}

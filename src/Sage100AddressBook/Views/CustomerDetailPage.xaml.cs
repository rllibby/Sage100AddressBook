/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.Views
{
    /// <summary>
    /// The code behind for the customer page.
    /// </summary>
    public sealed partial class CustomerDetailPage : Page
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public CustomerDetailPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        #endregion
    }
}

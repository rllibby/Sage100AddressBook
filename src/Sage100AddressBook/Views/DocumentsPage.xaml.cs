/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.Views
{
    /// <summary>
    /// Code behind for the Documents page.
    /// </summary>
    public sealed partial class DocumentsPage : Page
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DocumentsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        #endregion
    }
}


/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Sage100AddressBook.Views
{
    /// <summary>
    /// Code behind for the QuoteOrderPage.
    /// </summary>
    public sealed partial class QuoteOrderPage : Page
    {
        #region Private methods

        /// <summary>
        /// Page loaded event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var show = (ActualWidth >= 600);

            Items.Columns[2].IsVisible = show;
            Items.Columns[3].IsVisible = show;
            Items.Columns[4].IsVisible = show;
        }

        /// <summary>
        /// Visual state group changed event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CurrentVisualStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            var show = (!e.NewState.Name.Equals("VisualStateNarrow"));

            Items.Columns[2].IsVisible = show;
            Items.Columns[3].IsVisible = show;
            Items.Columns[4].IsVisible = show;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public QuoteOrderPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        #endregion
    }
}

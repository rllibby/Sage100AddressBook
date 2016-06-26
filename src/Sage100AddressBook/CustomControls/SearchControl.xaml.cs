/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Sage100AddressBook.CustomControls
{
    /// <summary>
    /// User control for displaying a search input box.
    /// </summary>
    public sealed partial class SearchControl : UserControl
    {
        #region Private fields

        private EventHandler _onSearch;
        private string _searchText;
        private bool _showing;

        #endregion

        #region Private methods

        /// <summary>
        /// Event that is triggered when a key is pressed in the text box.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSearchKey(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                _searchText = SearchBox.Text;
                _onSearch?.Invoke(this, new EventArgs());

                return;
            }

            e.Handled = false;
        }

        /// <summary>
        /// Event that is triggered when the text box loses focus.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDismissSearch(object sender, RoutedEventArgs e)
        {
            if (_showing && (Visibility == Visibility.Visible))
            {
                try
                {
                    Visibility = Visibility.Collapsed;
                    _showing = false;
                }
                finally
                {
                    _onSearch = null;
                }
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Override for the user control getting focus.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            SearchBox.Focus(FocusState.Programmatic);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SearchControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets focus to the search box
        /// </summary>
        /// <param name="onSearchCallback">The event handler to call when a search is executed.</param>
        public void ShowSearch(EventHandler onSearchCallback)
        {
            _onSearch = onSearchCallback;

            try
            {
                SearchBox.Text = string.Empty;
                Visibility = Visibility.Visible;
                Focus(FocusState.Programmatic);
            }
            finally
            {
                _showing = true;
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The search text that the user entered.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
        }

        #endregion
    }
}

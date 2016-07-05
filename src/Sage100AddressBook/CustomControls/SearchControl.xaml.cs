/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Sage100AddressBook.CustomControls
{
    /// <summary>
    /// Subclass for passing search result.
    /// </summary>
    public class SearchEventArgs : EventArgs
    {
        #region Private fields

        private readonly string _searchText;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="searchText">The search text results.</param>
        public SearchEventArgs(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) throw new ArgumentNullException("searchText");

            _searchText = searchText;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The search text.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
        }

        #endregion
    }

    /// <summary>
    /// User control for displaying a search input box.
    /// </summary>
    public sealed partial class SearchControl : UserControl
    {
        #region Private fields

        private EventHandler<SearchEventArgs> _onSearch;
        private string _searchText;
        private string _placeholderText;
        private bool _autoDismiss;
        private bool _active;
        private bool _showing;

        #endregion

        #region Private methods

        /// <summary>
        /// Event that is triggered when a key is pressed in the text box.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnSearchKey(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var criteria = SearchBox.Text;

                if (!string.IsNullOrEmpty(criteria))
                {
                    e.Handled = true;
                    
                    await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        _active = true;
                        _searchText = criteria;

                        SearchBox.PlaceholderText = criteria;

                        _onSearch?.Invoke(this, new SearchEventArgs(_searchText));
                    });

                    return;
                }
            }

            e.Handled = false;
        }

        /// <summary>
        /// Event that is triggered when the text box loses focus.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnDismissSearch(object sender, RoutedEventArgs e)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (_showing && (Visibility == Visibility.Visible) && !_active && _autoDismiss)
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
            });
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

            _autoDismiss = true;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets focus to the search box
        /// </summary>
        /// <param name="onSearchCallback">The event handler to call when a search is executed.</param>
        /// <param name="placeholderText">Optional placeholder text to show when text is empty.</param>
        public void ShowSearch(EventHandler<SearchEventArgs> onSearchCallback, string placeholderText = null)
        {
            _onSearch = onSearchCallback;

            try
            {
                SearchBox.Text = string.Empty;
                SearchBox.PlaceholderText = _placeholderText = (placeholderText == null) ? "Search for..." : placeholderText;
                Visibility = Visibility.Visible;
                Focus(FocusState.Programmatic);
            }
            finally
            {
                _showing = true;
            }
        }
        
        /// <summary>
        /// Resets the search box field.
        /// </summary>
        public void Reset()
        {
            SearchBox.Text = string.Empty;
            SearchBox.PlaceholderText = _placeholderText;
        }

        /// <summary>
        /// Close the search window.
        /// </summary>
        public async void CloseSearch()
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    Visibility = Visibility.Collapsed;
                }
                finally
                {
                    _active = false;
                }

            });
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

        /// <summary>
        /// True if the search 
        /// </summary>
        public bool AutoDismiss
        {
            get { return _autoDismiss; }
            set { _autoDismiss = value; }
        }

        #endregion
    }
}

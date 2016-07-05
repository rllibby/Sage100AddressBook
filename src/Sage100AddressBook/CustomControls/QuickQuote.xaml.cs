/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Microsoft.Graph;
using Sage100AddressBook.Helpers;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.CustomControls
{
    /// <summary>
    /// Content for a dialog that allows for quick quote creation.
    /// </summary>
    public sealed partial class QuickQuote : UserControl
    {
        #region Private fields

        private ObservableCollectionEx<string> _context = new ObservableCollectionEx<string>();
        private ContentDialog _dialog;
        private string _rootId;
        private int _selected = (-1);

        #endregion

        #region Private methods

        /// <summary>
        /// Sets or clears the busy state for the dialog.
        /// </summary>
        /// <param name="busy">True if busy, otherwise false.</param>
        private void SetBusy(bool busy)
        {
            if (busy)
            {
                Find.IsEnabled = false;
                SearchText.IsEnabled = false;
                Items.Visibility = Visibility.Collapsed;
                Progress.IsActive = true;

                return;
            }

            SearchText.IsEnabled = true;
            SearchText.Text = string.Empty;
            Progress.IsActive = false;
            Items.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Event that is triggered when a new group should be added.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void ItemFindClick(object sender, RoutedEventArgs e)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                SetBusy(true);

                var searchText = SearchText.Text;

                try
                {
                    /// TODO - query for an item.
                }
                catch (ServiceException exception)
                {
                    _dialog.Hide();

                    try
                    {
                        await Dialogs.ShowException(string.Format("Failed to match any items using '{0}'.", searchText), exception, false);
                        SearchText.Text = string.Empty;
                    }
                    finally
                    {
#pragma warning disable 4014
                        _dialog.ShowAsync();
#pragma warning restore 4014
                    }
                }
                finally
                {
                    SetBusy(false);
                }
            });
        }

        /// <summary>
        /// Event that is triggered when the search text changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Find.IsEnabled = (!string.IsNullOrEmpty(SearchText.Text));
            });
        }

        /// <summary>
        /// Event that is triggered when the selection changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _dialog.IsPrimaryButtonEnabled = (Items.SelectedIndex >= 0);
                _selected = Items.SelectedIndex;
            });
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public QuickQuote(ContentDialog dialog, string rootId)
        {
            InitializeComponent();

            if (dialog == null) throw new ArgumentNullException("dialog");
            if (string.IsNullOrEmpty(rootId)) throw new ArgumentException("rootId");

            Items.ItemsSource = _context;
            Find.IsEnabled = false;

            _dialog = dialog;
            _rootId = rootId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the selected item in the list.
        /// </summary>
        public int Selected
        {
            get { return _selected; }
        }

        /// <summary>
        /// Gets/sets the display text.
        /// </summary>
        public string DisplayText
        {
            get { return Display.Text; }
            set { Display.Text = value; }
        }

        #endregion
    }
}

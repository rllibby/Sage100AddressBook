﻿/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Microsoft.Graph;
using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.SearchServices;
using System;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Sage100AddressBook.CustomControls
{
    /// <summary>
    /// Content for a dialog that allows for quick quote creation.
    /// </summary>
    public sealed partial class QuickQuote : UserControl
    {
        #region Private fields

        private ObservableCollectionEx<QuickQuoteLine> _context = new ObservableCollectionEx<QuickQuoteLine>();
        private ContentDialog _dialog;
        private QuickQuoteLine _selected;
        private string _companyId;
        private string _customerId;

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
        /// Event that is triggered when the quantity changes for a quick quote line item.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="arg">The event arguments.</param>
        private void OnQuantityChanged(object sender, EventArgs arg)
        {
            var entry = Items.SelectedItem as QuickQuoteLine;

            if (entry == null) return;

            _dialog.IsPrimaryButtonEnabled = (entry.Quantity >= 1);
        }

        /// <summary>
        /// Query and return the recommended items.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void RecommendedClick(object sender, RoutedEventArgs e)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                SetBusy(true);

                _context.Clear();

                try
                {
                    var items = await ItemSearchService.Instance.ExecuteRecommendedAsync(_companyId, _customerId);

                    foreach (var item in items)
                    {
                        var entry = new QuickQuoteLine
                        {
                            Quantity = (item.QuantityToBuy > 0) ? (int)item.QuantityToBuy : 1,
                            Id = item.Id,
                            Description = item.ItemCodeDesc,
                        };

                        entry.QuantityChanged += OnQuantityChanged;
                        _context.Add(entry);

                    }
                }
                catch (ServiceException exception)
                {
                    _dialog.Hide();

                    try
                    {
                        await Dialogs.ShowException("Failed to load recommended items.", exception, false);
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
        /// Perform the item look up.
        /// </summary>
        /// <param name="searchText">The search text to use for locating the item.</param>
        /// <returns>The async task to wait on.</returns>
        private async Task LookupItem(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return;

            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                SetBusy(true);

                _context.Clear();

                try
                {
                    var items = await ItemSearchService.Instance.ExecuteSearchAsync(_companyId, searchText);

                    foreach (var item in items)
                    {
                        var entry = new QuickQuoteLine
                        {
                            Quantity = (item.QuantityToBuy > 0) ? (int)item.QuantityToBuy : 1,
                            Id = item.Id,
                            Description = item.ItemCodeDesc,
                        };

                        entry.QuantityChanged += OnQuantityChanged;
                        _context.Add(entry);
                    }
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
        /// Event that is triggered when a new group should be added.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void ItemFindClick(object sender, RoutedEventArgs e)
        {
            await LookupItem(SearchText.Text);
        }

        /// <summary>
        /// Event that is triggered when a key down event occurs in the text field.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnSearchKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                await LookupItem(SearchText.Text);

                return;
            }

            e.Handled = false;
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
        private async void OnSelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var selected = Items.SelectedItem as QuickQuoteLine;

                _dialog.IsPrimaryButtonEnabled = ((selected != null) && (selected.Quantity > 0));
                _selected = selected;
            });
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public QuickQuote(ContentDialog dialog, string companyId, string customerId)
        {
            InitializeComponent();

            if (dialog == null) throw new ArgumentNullException("dialog");
            if (string.IsNullOrEmpty(companyId)) throw new ArgumentNullException("companyId");
            if (string.IsNullOrEmpty(customerId)) throw new ArgumentNullException("customerId");

            Items.ItemsSource = _context;
            Find.IsEnabled = false;

            _dialog = dialog;
            _companyId = companyId;
            _customerId = customerId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the selected item in the list.
        /// </summary>
        public QuickQuoteLine Selected
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

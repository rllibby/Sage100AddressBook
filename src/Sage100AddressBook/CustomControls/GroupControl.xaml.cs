﻿/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Microsoft.Graph;
using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.CustomControls
{
    /// <summary>
    /// Content for a dialog that allows for group creation / selection.
    /// </summary>
    public sealed partial class GroupControl : UserControl
    {
        #region Private fields

        private ObservableCollectionEx<DocumentFolder> _context = new ObservableCollectionEx<DocumentFolder>();
        private ICollection<DocumentFolder> _source;
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
                Add.IsEnabled = false;
                Group.IsEnabled = false;
                Items.Visibility = Visibility.Collapsed;
                Progress.IsActive = true; 

                return;
            }

            Group.IsEnabled = true;
            Group.Text = string.Empty;
            Progress.IsActive = false;
            Items.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Event that is triggered when a new group should be added.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void GroupAddClick(object sender, RoutedEventArgs e)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var groupName = Group.Text;

                SetBusy(true);

                try
                {
                    var driveItem = new DriveItem()
                    {
                        Name = groupName,
                        Folder = new Folder()
                    };

                    var client = await AuthenticationHelper.GetClient();
                    var folder = await client?.Me.Drive.Root.ItemWithPath(_rootId).Children.Request().AddAsync(driveItem);

                    if (folder != null)
                    {
                        var newFolder = new DocumentFolder(folder.Id, folder.Name);

                        _source.Add(newFolder);
                        _context.Add(newFolder);
                        Items.SelectedIndex = Items.Items.Count - 1;
                    }
                }
                catch (ServiceException exception)
                {
                    _dialog.Hide();

                    try
                    {
                        await Dialogs.ShowException(string.Format("Failed to create the group '{0}'.", groupName), exception, false);
                        Group.Text = string.Empty;
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
        /// Event that is triggered when the group text changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnGroupTextChanged(object sender, TextChangedEventArgs e)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
            {
                if (string.IsNullOrEmpty(Group.Text))
                {
                    Add.IsEnabled = false;
                    return;
                }

                var group = Group.Text;
                var match = _source.FirstOrDefault(f => f.Name.Equals(group, StringComparison.OrdinalIgnoreCase));

                Add.IsEnabled = (match == null);
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
        public GroupControl(ContentDialog dialog, ICollection<DocumentFolder> source, string rootId)
        {
            InitializeComponent();

            if (dialog == null) throw new ArgumentNullException("dialog");
            if (source == null) throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(rootId)) throw new ArgumentException("rootId");

            Items.ItemsSource = _context;
            Add.IsEnabled = false;

            _dialog = dialog;
            _rootId = rootId;
            _source = source;

            _context.Set(source);
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

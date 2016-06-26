/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.CustomControls
{
    /// <summary>
    /// List box for selecting items in a content dialog.
    /// </summary>
    public sealed partial class ListControl : UserControl
    {
        #region Private fields

        private int _selected = (-1);

        #endregion

        #region Private methods

        /// <summary>
        /// Event that is fired when the selection changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (sender as ListBox);

            _selected = listBox.SelectedIndex;

            SelectionChanged?.Invoke(sender, new EventArgs());
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ListControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Event handler for the selection changed event.
        /// </summary>
        public EventHandler SelectionChanged; 

        /// <summary>
        /// The items source for the control.
        /// </summary>
        public object ItemsSource
        {
            get { return SelectionList.ItemsSource; }
            set { SelectionList.ItemsSource = value; }
        }

        /// <summary>
        /// Returns the index of the selected item.
        /// </summary>
        public int Selected
        {
            get { return _selected; }
        }

        #endregion
    }
}

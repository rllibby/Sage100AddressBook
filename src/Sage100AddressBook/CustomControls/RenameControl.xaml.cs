/*
 *  Copyright © 2016, Russell Libby 
 */

using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.CustomControls
{
    /// <summary>
    /// Content for rename dialog.
    /// </summary>
    public sealed partial class RenameControl : UserControl
    {
        #region Private fields

        private ContentDialog _dialog;
        private string _original;

        #endregion

        #region Private methods

        /// <summary>
        /// Event that is triggered when the text changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event argument.</param>
        private async void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (string.IsNullOrEmpty(Rename.Text) || string.Equals(Rename.Text, _original, System.StringComparison.OrdinalIgnoreCase))
                {
                    _dialog.IsPrimaryButtonEnabled = false;
                    return;
                }

                _dialog.IsPrimaryButtonEnabled = true;
            });
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dialog">The dialog hosting the control.</param>
        /// <param name="original">The starting string for the rename.</param>
        public RenameControl(ContentDialog dialog, string original)
        {
            InitializeComponent();

            if (dialog == null) throw new ArgumentNullException("dialog");

            _dialog = dialog;
            _dialog.IsPrimaryButtonEnabled = false;
            _original = original;

            Rename.Text = original;
            Rename.PlaceholderText = original;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The renamed text.
        /// </summary>
        public string RenameText
        {
            get { return Rename.Text; }
        }

        #endregion
    }
}

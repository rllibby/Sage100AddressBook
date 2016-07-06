/*
 *  Copyright © 2016, Russell Libby 
 */

using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Sage100AddressBook.CustomControls
{
    /// <summary>
    /// Content for input text dialog.
    /// </summary>
    public sealed partial class InputBox : UserControl
    {
        #region Private fields

        private ContentDialog _dialog;

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
                _dialog.IsPrimaryButtonEnabled = (!string.IsNullOrEmpty(InputText.Text));
            });
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dialog">The dialog hosting the control.</param>
        /// <param name="title">The title for the input.</param>
        /// <param name="value">The starting string for the input.</param>
        /// <param name="placeholder">The optional placeholder text to display.</param>
        public InputBox(ContentDialog dialog, string title, string value = null, string placeholder = null)
        {
            InitializeComponent();

            if (dialog == null) throw new ArgumentNullException("dialog");
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException("title");

            _dialog = dialog;
            _dialog.IsPrimaryButtonEnabled = false;

            Display.Text = title;
            InputText.Text = value ?? string.Empty;
            InputText.PlaceholderText = placeholder ?? string.Empty;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The resulting text.
        /// </summary>
        public string ResultText
        {
            get { return InputText.Text; }
        }

        /// <summary>
        /// Input scope for text box.
        /// </summary>
        public InputScope Scope
        {
            get { return InputText.InputScope; }
            set { InputText.InputScope = value; }
        } 

        #endregion
    }
}

/*
 *  Copyright © 2016, Russell Libby 
 */

using System;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.CustomControls
{
    /// <summary>
    /// Numeric input box.
    /// </summary>
    public sealed partial class NumericInputBox : UserControl
    {
        #region Private fields

        private ContentDialog _dialog;

        #endregion

        #region Private methods

        /// <summary>
        /// Event that is fired when the value changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnValueChanged(object sender, EventArgs e)
        {
            InputText.Value = (InputText.Value == null) ? 0 : Convert.ToInt32(InputText.Value);

            _dialog.IsPrimaryButtonEnabled = (InputText.Value >= 0);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dialog">The dialog hosting the control.</param>
        /// <param name="title">The title for the input.</param>
        /// <param name="value">The starting numeric value for the input.</param>
        public NumericInputBox(ContentDialog dialog, string title, int value = 0)
        {
            InitializeComponent();

            if (dialog == null) throw new ArgumentNullException("dialog");
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException("title");

            _dialog = dialog;
            _dialog.IsPrimaryButtonEnabled = false;

            Display.Text = title;
            InputText.ValueFormat = "{0:N0}";
            InputText.Value = value;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the current value.
        /// </summary>
        public int Value
        {
            get { return Convert.ToInt32(InputText.Value); }
        }

        #endregion
    }
}

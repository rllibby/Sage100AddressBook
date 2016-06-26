/*
 *  Copyright © 2016, Russell Libby 
 */

using Sage100AddressBook.CustomControls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Static class to simplify dialog handling.
    /// </summary>
    public static class Dialogs
    {
        #region Private constants

        private const string Title = "Sage 100 Address Book";
        private const string Ok = "OK";
        private const string Cancel = "Cancel";

        #endregion

        #region Private methods

        /// <summary>
        /// Command handlers for the ok dialog.
        /// </summary>
        /// <param name="commandLabel">The command selected by the user.</param>
        private static void CommandOk(IUICommand commandLabel) { }

        /// <summary>
        /// Command handlers for the ok dialog.
        /// </summary>
        /// <param name="commandLabel">The command selected by the user.</param>
        private static void CommandCancel(IUICommand commandLabel) { }

        #endregion

        #region Public methods

        /// <summary>
        /// Show a selection list dialog.
        /// </summary>
        /// <param name="header">The title for the dialog.</param>
        /// <param name="items">The collection of items to show in list.</param>
        /// <returns>The index of the selected item.</returns>
        public static async Task<int> ShowSelection(string header, ICollection<object> items)
        {
            if ((items == null) || (items.Count == 0)) return (-1);

            var dialog = new ContentDialog()
            {
                Title = header,
                MaxWidth = Math.Min(300, Window.Current.Content.RenderSize.Width - 100),
                MaxHeight = Math.Min(400, Window.Current.Content.RenderSize.Height - 100)
            };

            var control = new ListControl
            {
                ItemsSource = items,
                Margin = new Thickness(10, 10, 10, 10) 
            };

            control.SelectionChanged += delegate (object sender, EventArgs e)
            {
                dialog.IsPrimaryButtonEnabled = (control.Selected >= 0);
            };

            dialog.Content = control;

            var result = (-1);

            dialog.PrimaryButtonText = Ok;
            dialog.SecondaryButtonText = Cancel;
            dialog.IsPrimaryButtonEnabled = false;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonClick += delegate { result = control.Selected; };
            dialog.SecondaryButtonClick += delegate { result = (-1); };

            await dialog.ShowAsync();

            return result;
        }

        /// <summary>
        /// Show exception dialog.
        /// </summary>
        /// <param name="header">The title for the dialog.</param>
        /// <param name="exception">The exception information.</param>
        /// <param name="allowCancel">True if the cancel button should be enabled.</param>
        /// <returns></returns>
        public static async Task<bool> ShowException(string header, Exception exception, bool allowCancel = true)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            var dialog = new ContentDialog()
            {
                Title = Title,
                MaxWidth = Window.Current.Content.RenderSize.Width - 40,
                MaxHeight = Window.Current.Content.RenderSize.Height - 40
            };

            var temp = new StringBuilder();

            temp.AppendLine(header);
            temp.AppendLine();
            temp.AppendLine(exception.ToString());

            var viewer = new ScrollViewer
            {
                Margin = new Thickness(8, 8, 8, 8),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                MaxHeight = 400,
                MaxWidth = Window.Current.Content.RenderSize.Width - 80,
            };

            var text = new TextBlock
            {
                Text = temp.ToString(),
                TextWrapping = TextWrapping.Wrap,
            };

            viewer.Content = text;
            dialog.Content = viewer;

            bool result = false;

            dialog.PrimaryButtonText = Ok;
            dialog.SecondaryButtonText = Cancel;
            dialog.IsPrimaryButtonEnabled = true;
            dialog.IsSecondaryButtonEnabled = allowCancel;
            dialog.PrimaryButtonClick += delegate { result = true; };
            dialog.SecondaryButtonClick += delegate { result = false; };

            await dialog.ShowAsync();

            return result;
        }

        /// <summary>
        /// Shows a dialog to the user.
        /// </summary>
        /// <param name="message">The message to display in the dialog.</param>
        /// <returns></returns>
        public static async Task Show(string message)
        {
            var dialog = new MessageDialog(message, Title);

            dialog.Commands.Add(new UICommand(Ok, CommandOk));

            await dialog.ShowAsync().AsTask();
        }

        /// <summary>
        /// Shows a dialog to the user.
        /// </summary>
        /// <param name="message">The message to display in the dialog.</param>
        /// <returns></returns>
        public static async Task<bool> ShowOkCancel(string message)
        {
            var dialog = new MessageDialog(message, Title);

            dialog.Commands.Add(new UICommand(Ok, CommandOk, 1));
            dialog.Commands.Add(new UICommand(Cancel, CommandCancel, 0));

            var command = await dialog.ShowAsync().AsTask();

            return command.Id.Equals(1);
        }

        #endregion
    }
}

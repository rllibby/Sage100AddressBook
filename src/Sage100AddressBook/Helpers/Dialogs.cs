/*
 *  Copyright © 2016, Russell Libby 
 */

using System;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
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
        /// Show exception dialog.
        /// </summary>
        /// <param name="title">The title for the dialog.</param>
        /// <param name="exception">The exception information.</param>
        /// <param name="allowCancel">True if the cancel button should be enabled.</param>
        /// <returns></returns>
        public static async Task<bool> ShowException(string header, Exception exception, bool allowCancel = true)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            var dialog = new ContentDialog()
            {
                Title = ApplicationView.GetForCurrentView().Title,
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
        /// <param name="title">The title for the dialog.</param>
        /// <param name="message">The message to display in the dialog.</param>
        /// <returns></returns>
        public static async Task Show(string title, string message)
        {
            var dialog = new MessageDialog(message, title);

            dialog.Commands.Add(new UICommand(Ok, CommandOk));

            await dialog.ShowAsync().AsTask();
        }

        /// <summary>
        /// Shows a dialog to the user.
        /// </summary>
        /// <param name="title">The title for the dialog.</param>
        /// <param name="message">The message to display in the dialog.</param>
        /// <returns></returns>
        public static async Task<bool> ShowOkCancel(string title, string message)
        {
            var dialog = new MessageDialog(message, title);

            dialog.Commands.Add(new UICommand(Ok, CommandOk, 1));
            dialog.Commands.Add(new UICommand(Cancel, CommandCancel, 0));

            var command = await dialog.ShowAsync().AsTask();

            return command.Id.Equals(1);
        }

        #endregion
    }
}

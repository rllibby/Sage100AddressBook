/*
 *  Copyright © 2016, Russell Libby 
 */

using Sage100AddressBook.CustomControls;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.SettingsServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Enumeration of group operations.
    /// </summary>
    public enum GroupOperation
    {
        /// <summary>
        /// Upload to group.
        /// </summary>
        Upload,

        /// <summary>
        /// Move to group.
        /// </summary>
        Move,

        /// <summary>
        /// Copy to group.
        /// </summary>
        Copy
    } 

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
        /// Shows a dialog that allows for text based user input.
        /// </summary>
        /// <param name="scope">The input scope for the text box.</param>
        /// <param name="title">The title for the input box.</param>
        /// <param name="value">The optional starting value.</param>
        /// <param name="placeholder">The optional placeholder text for the input box.</param>
        /// <returns>The resulting string on success, null on cancel.</returns>
        public static async Task<string> Input(InputScope scope, string title, string value = null, string placeholder = null)
        {
            var dialog = new ContentDialog()
            {
                Title = string.Empty,
                MaxWidth = Math.Min(400, App.Bounds.Width - 2),
                MaxHeight = 210,
            };

            var control = new InputBox(dialog, title, value, placeholder)
            {
                Scope = scope,
                Background = dialog.Background,
                Width = dialog.MaxWidth - 40,
                Height = dialog.MaxHeight - 120,
            };

            dialog.RequestedTheme = SettingsService.Instance.AppTheme.ToElementTheme();
            dialog.Content = control;

            var result = (string)null;

            dialog.PrimaryButtonText = Ok;
            dialog.SecondaryButtonText = Cancel;
            dialog.IsPrimaryButtonEnabled = true;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonClick += delegate { result = control.ResultText; };
            dialog.SecondaryButtonClick += delegate { result = null; };

            await dialog.ShowAsync();

            return result;
        }

        /// <summary>
        /// Shows a dialog that allows a rename to occur.
        /// </summary>
        /// <param name="original">The original string for the rename.</param>
        /// <returns>The renamed string on success, null on cancel.</returns>
        public static async Task<string> Rename(string original)
        {
            var dialog = new ContentDialog()
            {
                Title = string.Empty,
                MaxWidth = Math.Min(400, App.Bounds.Width - 2),
                MaxHeight = 210,
            };

            var control = new RenameControl(dialog, original)
            {
                Background = dialog.Background,
                Width = dialog.MaxWidth - 40,
                Height = dialog.MaxHeight - 120,
            };

            dialog.RequestedTheme = SettingsService.Instance.AppTheme.ToElementTheme();
            dialog.Content = control;

            var result = (string)null;

            dialog.PrimaryButtonText = Ok;
            dialog.SecondaryButtonText = Cancel;
            dialog.IsPrimaryButtonEnabled = true;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonClick += delegate { result = control.RenameText; };
            dialog.SecondaryButtonClick += delegate { result = null; };

            await dialog.ShowAsync();

            return result;
        }

        /// <summary>
        /// Show a selection dialog for share link types.
        /// </summary>
        /// <returns>The index of the selected item.</returns>
        public static async Task<int> SelectLink()
        {
            var dialog = new ContentDialog()
            {
                Title = string.Empty,
                MaxWidth = Math.Min(300, App.Bounds.Width - 2),
                MaxHeight = 260,
            };

            var control = new LinkTypeControl()
            {
                Background = dialog.Background,
                Width = dialog.MaxWidth - 40,
                Height = dialog.MaxHeight - 120,
            };

            dialog.RequestedTheme = SettingsService.Instance.AppTheme.ToElementTheme();
            dialog.Content = control;

            var result = (-1);

            dialog.PrimaryButtonText = Ok;
            dialog.SecondaryButtonText = Cancel;
            dialog.IsPrimaryButtonEnabled = true;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonClick += delegate { result = control.Selected; };
            dialog.SecondaryButtonClick += delegate { result = (-1); };

            await dialog.ShowAsync();

            return result;
        }

        /// <summary>
        /// Show a selection list dialog.
        /// </summary>
        /// <param name="companyId">The company identifier.<param>
        /// <param name="customerId">The customer identifier.<param>
        /// <returns>The QuickQuoteLine on success, null on failure.</returns>
        public static async Task<QuickQuoteLine> GetQuickQuoteItem(string companyId, string customerId)
        {
            if (string.IsNullOrEmpty(companyId) || string.IsNullOrEmpty(customerId)) return (null);

            var dialog = new ContentDialog()
            {
                Title = string.Empty,
                Name = "Dialog",
                MaxWidth = Math.Min(600, App.Bounds.Width - 2),
                MaxHeight = Math.Min(420, Window.Current.Bounds.Height - 20),
            };

            var control = new CustomControls.QuickQuote(dialog, companyId, customerId)
            {
                DisplayText = "New Quick Quote",
                Background = dialog.Background,
                Width = dialog.MaxWidth - 40,
                Height = dialog.MaxHeight - 120,
            };

            dialog.RequestedTheme = SettingsService.Instance.AppTheme.ToElementTheme();
            dialog.Content = control;

            QuickQuoteLine result = null;

            dialog.PrimaryButtonText = Ok;
            dialog.SecondaryButtonText = Cancel;
            dialog.IsPrimaryButtonEnabled = false;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonClick += delegate { result = control.Selected; };
            dialog.SecondaryButtonClick += delegate { result = null; };

            await dialog.ShowAsync();

            if (result != null)
            {
                var ok = await ShowOkCancel(string.Format("Create a new quick quote for:\n\n({0}) - {1}", result.Quantity, result.Description));

                if (!ok) result = null;
            }

            return result;
        }

        /// <summary>
        /// Show a selection list dialog.
        /// </summary>
        /// <param name="operation">The operation to perform, this controls the text.</param>
        /// <param name="items">The collection of items to show in list.</param>
        /// <param name="rootId">The base level folder name<param>
        /// <returns>The index of the selected item.</returns>
        public static async Task<int> SelectGroup(GroupOperation operation, ICollection<DocumentFolder> items, string rootId)
        {
            if ((items == null) || (items.Count == 0) || string.IsNullOrEmpty(rootId)) return (-1);

            var dialog = new ContentDialog()
            {
                Title = string.Empty,
                MaxWidth = Math.Min(400, App.Bounds.Width - 2),
                MaxHeight = Math.Min(400, Window.Current.Bounds.Height - 100),
            };

            var control = new GroupControl(dialog, items, rootId)
            {
                DisplayText = string.Format("Select a group to {0} to.", operation.ToString().ToLower()),
                Background = dialog.Background,
                Width = dialog.MaxWidth - 40,
                Height = dialog.MaxHeight - 160,
            };

            dialog.RequestedTheme = SettingsService.Instance.AppTheme.ToElementTheme();
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
                MaxWidth = App.Bounds.Width - 2,
                MaxHeight = Window.Current.Bounds.Height - 40
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
                MaxWidth = Window.Current.Bounds.Width - 80,
            };

            var text = new TextBlock
            {
                Text = temp.ToString(),
                TextWrapping = TextWrapping.Wrap,
            };

            viewer.Content = text;
            dialog.Content = viewer;
            dialog.RequestedTheme = SettingsService.Instance.AppTheme.ToElementTheme();

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
            var dialog = new ContentDialog()
            {
                Title = string.Empty,
                MaxWidth = Math.Min(400, App.Bounds.Width - 2),
                MaxHeight = 200,
            };

            var control = new TextBlock
            {
                Text = message,
                Width = dialog.MaxWidth - 40,
                Height = dialog.MaxHeight - 120,
                TextWrapping = TextWrapping.Wrap,
                MaxLines = 3
            };

            dialog.Content = control;
            dialog.RequestedTheme = SettingsService.Instance.AppTheme.ToElementTheme();

            dialog.PrimaryButtonText = Ok;
            dialog.SecondaryButtonText = Cancel;
            dialog.IsPrimaryButtonEnabled = true;
            dialog.IsSecondaryButtonEnabled = false;

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Shows a dialog to the user.
        /// </summary>
        /// <param name="message">The message to display in the dialog.</param>
        /// <returns></returns>
        public static async Task<bool> ShowOkCancel(string message)
        {
            var dialog = new ContentDialog()
            {
                Title = string.Empty,
                MaxWidth = Math.Min(400, App.Bounds.Width - 2),
                MaxHeight = 200,
            };

            var control = new TextBlock
            {
                Text = message,
                Width = dialog.MaxWidth - 40,
                Height = dialog.MaxHeight - 120,
                TextWrapping = TextWrapping.Wrap,
                MaxLines = 3
            };

            dialog.Content = control;
            dialog.RequestedTheme = SettingsService.Instance.AppTheme.ToElementTheme();

            var result = false;

            dialog.PrimaryButtonText = Ok;
            dialog.SecondaryButtonText = Cancel;
            dialog.IsPrimaryButtonEnabled = true;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonClick += delegate { result = true; };
            dialog.SecondaryButtonClick += delegate { result = false; };

            await dialog.ShowAsync();

            return result;
        }

        #endregion
    }
}

/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Template10.Common;
using Template10.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.Views
{
    /// <summary>
    /// User control for busy modal dialog.
    /// </summary>
    public sealed partial class Busy : UserControl
    {
        #region Constructor.

        /// <summary>
        /// Constructor.
        /// </summary>
        public Busy()
        {
            InitializeComponent();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Shows or hides the busy modal window.
        /// </summary>
        /// <param name="busy">True to show, false to hide.</param>
        /// <param name="text">The text to display next to the progress ring.</param>
        public static void SetBusy(bool busy, string text = null)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                var modal = Window.Current.Content as ModalDialog;
                var view = modal.ModalContent as Busy;

                if (view == null) modal.ModalContent = view = new Busy();
                modal.IsModal = view.IsBusy = busy;
                view.BusyText = text;
            });
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The text to display.
        /// </summary>
        public string BusyText
        {
            get { return (string)GetValue(BusyTextProperty); }
            set { SetValue(BusyTextProperty, value); }
        }

        /// <summary>
        /// Dependency property for busy text.
        /// </summary>
        public static readonly DependencyProperty BusyTextProperty = DependencyProperty.Register(nameof(BusyText), typeof(string), typeof(Busy), new PropertyMetadata("Please wait..."));

        /// <summary>
        /// True if busy, otherwise false.
        /// </summary>
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        /// <summary>
        /// Dependency property for is busy.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(Busy), new PropertyMetadata(false));

        #endregion
    }
}


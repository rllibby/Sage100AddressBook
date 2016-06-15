/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.Views
{
    /// <summary>
    /// Shell page for hosting page content within the split view.
    /// </summary>
    public sealed partial class Shell : Page
    {
        #region Private methods

        /// <summary>
        /// Event that is triggered when the hamburger menu is loaded.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The routed event arguments.</param>
        private void MyHamburgerMenu_Loaded(object sender, RoutedEventArgs e)
        {
            HamburgerMenu.IsOpen = (Window.Current.Bounds.Width > 520);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public Shell()
        {
            Instance = this;
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="navigationService">The navigation service to initialize with.</param>
        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the navigation service for the shell.
        /// </summary>
        /// <param name="navigationService">The navigation service to set.</param>
        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Instance of the shell.
        /// </summary>
        public static Shell Instance { get; set; }

        /// <summary>
        /// Instance of the hamburger menu within the shell.
        /// </summary>
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;

        #endregion
    }
}


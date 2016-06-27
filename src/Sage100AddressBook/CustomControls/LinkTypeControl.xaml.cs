
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.CustomControls
{
    /// <summary>
    /// Link type content for share dialog.
    /// </summary>
    public sealed partial class LinkTypeControl : UserControl
    {
        #region Private fields

        private int _selected;

        #endregion

        #region Private methods

        /// <summary>
        /// Event that is triggered when one of the radio buttons is checked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event argument.</param>
        private void OnItemChecked(object sender, RoutedEventArgs e)
        {
            var radio = (sender as RadioButton);

            _selected = (radio == View) ? 0 : 1;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public LinkTypeControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the selected item.
        /// </summary>
        public int Selected
        {
            get { return _selected; }
        }

        #endregion
    }
}
